using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;

namespace ChucheriasWeb.Pages
{
    public class ProductoGolf
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Tipo { get; set; }
        public string Marca { get; set; }
        public string ImagenUrl { get; set; }
    }

    public class ProductosGolfModel : PageModel
    {
        public List<ProductoGolf> Catalogo { get; set; } = new();
        public List<string> Tipos { get; set; } = new();

        [BindProperty]
        public string NuevoNombre { get; set; }
        [BindProperty]
        public string NuevoTipo { get; set; }
        [BindProperty]
        public string NuevaMarca { get; set; }

        public async Task OnGetAsync()
        {
            await CargarProductosAsync();
            await CargarTiposAsync();
        }

        public async Task<IActionResult> OnPostAgregarAsync()
        {
            using var client = new HttpClient();
            var nuevo = new ProductoGolf { Nombre = NuevoNombre, Tipo = NuevoTipo, Marca = NuevaMarca };
            var json = JsonSerializer.Serialize(nuevo);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            await client.PostAsync("https://localhost:7027/ProductosGolf/agregar", content);
            return RedirectToPage();
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
                Catalogo = JsonSerializer.Deserialize<List<ProductoGolf>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
                foreach (var producto in Catalogo)
                {
                    producto.ImagenUrl = producto.Tipo switch
                    {
                        "Palo" => "https://images.unsplash.com/photo-1517649763962-0c623066013b?auto=format&fit=crop&w=400&q=80",
                        "Bola" => "https://images.unsplash.com/photo-1506744038136-46273834b3fb?auto=format&fit=crop&w=400&q=80",
                        "Guante" => "https://images.unsplash.com/photo-1526178613658-3f1622045557?auto=format&fit=crop&w=400&q=80",
                        _ => "https://images.unsplash.com/photo-1465101046530-73398c7f28ca?auto=format&fit=crop&w=400&q=80"
                    };
                }
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
