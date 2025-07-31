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
        public List<string> CategoriasFijas { get; set; } = new() { "Palos", "Bolas", "Bolsas", "Ropa", "Accesorios" };
        public List<ArticuloGolf> ProductosPorCategoria { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(string categoria = null)
        {
            if (!string.IsNullOrEmpty(categoria))
            {
                await CargarProductosPorCategoriaAsync(categoria);
            }
            return Page();
        }

        private async Task CargarProductosPorCategoriaAsync(string categoria)
        {
            using var client = new HttpClient();
            var response = await client.GetAsync($"https://localhost:7027/ProductosGolf/porcategoria/{categoria}");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                ProductosPorCategoria = JsonSerializer.Deserialize<List<ArticuloGolf>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
            }
        }
    }
}
