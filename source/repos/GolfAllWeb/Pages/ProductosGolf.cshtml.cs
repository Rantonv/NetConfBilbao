using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;

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

        public async Task OnGetAsync()
        {
            await CargarProductosAsync();
            await CargarTiposAsync();
        }

        public async Task<IActionResult> OnPostEliminarAsync(int id)
        {
            using var client = new HttpClient();
            await client.DeleteAsync($"https://localhost:7027/ProductosGolf/eliminar/{id}");
            return RedirectToPage();
        }

        private async Task CargarProductosAsync()
        {
            using var client = new HttpClient();
            var response = await client.GetAsync("https://localhost:7027/ProductosGolf/catalogo");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                Catalogo = JsonSerializer.Deserialize<List<ArticuloGolf>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
                // Ya no sobrescribir ImagenUrl, usar la que viene del backend
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
    }
}
