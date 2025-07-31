using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace ChucheriasWeb.Pages
{
    public class ArticuloGolf
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Tipo { get; set; }
        public string Marca { get; set; }
        public string ImagenUrl { get; set; }
    }

    public class ProductosGolfModel : PageModel
    {
        public List<ArticuloGolf> Catalogo { get; set; } = new();
        public List<string> Tipos { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string SearchTerm { get; set; }

        public async Task OnGetAsync()
        {
            await CargarProductosAsync();
            await CargarTiposAsync();
            FiltrarCatalogo();
        }

        public async Task<IActionResult> OnPostEliminarAsync(int id)
        {
            using var client = new HttpClient();
            await client.DeleteAsync($"https://localhost:7027/ProductosGolf/eliminar/{id}");
            return RedirectToPage(new { SearchTerm });
        }

        private async Task CargarProductosAsync()
        {
            using var client = new HttpClient();
            var response = await client.GetAsync("https://localhost:7027/ProductosGolf/catalogo");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                Catalogo = JsonSerializer.Deserialize<List<ArticuloGolf>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
            }
        }

        private async Task CargarTiposAsync()
        {
            using var client = new HttpClient();
            var response = await client.GetAsync("https://localhost:7027/ProductosGolf/tipos");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                Tipos = JsonSerializer.Deserialize<List<string>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
            }
        }

        private void FiltrarCatalogo()
        {
            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                var term = SearchTerm.ToLowerInvariant();
                Catalogo = Catalogo.Where(p =>
                    (!string.IsNullOrEmpty(p.Nombre) && p.Nombre.ToLowerInvariant().Contains(term)) ||
                    (!string.IsNullOrEmpty(p.Tipo) && p.Tipo.ToLowerInvariant().Contains(term)) ||
                    (!string.IsNullOrEmpty(p.Marca) && p.Marca.ToLowerInvariant().Contains(term))
                ).ToList();
            }
        }
    }
}
