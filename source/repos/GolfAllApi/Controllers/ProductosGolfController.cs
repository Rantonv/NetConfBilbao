using Microsoft.AspNetCore.Mvc;
using GolfAllApi.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.IO;
using System.Diagnostics;

namespace GolfAllApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductosGolfController : ControllerBase
    {
        // Diccionario de productos por categoría
        private static readonly Dictionary<string, List<ArticuloGolf>> _productosPorCategoria = new()
        {
            ["Palos"] = new List<ArticuloGolf>
            {
                new ArticuloGolf { Id = 1, Nombre = "Palo de golf Pro", Tipo = "Palos", Marca = "Callaway", ImagenUrl = "https://images.unsplash.com/photo-1519864600265-abb23847ef2c?auto=format&fit=crop&w=400&q=80" },
                new ArticuloGolf { Id = 2, Nombre = "Palo de golf Avanzado", Tipo = "Palos", Marca = "TaylorMade", ImagenUrl = "https://images.unsplash.com/photo-1519864600265-abb23847ef2c?auto=format&fit=crop&w=400&q=80" }
            },
            ["Bolas"] = new List<ArticuloGolf>
            {
                new ArticuloGolf { Id = 3, Nombre = "Bola Premium", Tipo = "Bolas", Marca = "Titleist", ImagenUrl = "https://images.unsplash.com/photo-1506744038136-46273834b3fb?auto=format&fit=crop&w=400&q=80" },
                new ArticuloGolf { Id = 4, Nombre = "Bola Soft", Tipo = "Bolas", Marca = "Srixon", ImagenUrl = "https://images.unsplash.com/photo-1506744038136-46273834b3fb?auto=format&fit=crop&w=400&q=80" }
            },
            ["Bolsas"] = new List<ArticuloGolf>
            {
                new ArticuloGolf { Id = 5, Nombre = "Bolsa Stand", Tipo = "Bolsas", Marca = "Ping", ImagenUrl = "https://images.unsplash.com/photo-1464983953574-0892a716854b?auto=format&fit=crop&w=400&q=80" }
            },
            ["Ropa"] = new List<ArticuloGolf>
            {
                new ArticuloGolf { Id = 6, Nombre = "Polo Golf", Tipo = "Ropa", Marca = "Nike", ImagenUrl = "https://images.unsplash.com/photo-1512436991641-6745cdb1723f?auto=format&fit=crop&w=400&q=80" }
            },
            ["Accesorios"] = new List<ArticuloGolf>
            {
                new ArticuloGolf { Id = 7, Nombre = "Guante Soft", Tipo = "Accesorios", Marca = "FootJoy", ImagenUrl = "https://images.unsplash.com/photo-1519125323398-675f0ddb6308?auto=format&fit=crop&w=400&q=80" }
            }
        };

        [HttpGet("porcategoria/{categoria}")]
        public ActionResult<IEnumerable<ArticuloGolf>> GetPorCategoria(string categoria)
        {
            if (_productosPorCategoria.TryGetValue(categoria, out var productos))
                return Ok(productos);
            return Ok(new List<ArticuloGolf>());
        }

        private static readonly string _filePath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "productos_golf.json");
        private static List<ArticuloGolf> _productos = LoadProductos();

        private static List<ArticuloGolf> LoadProductos()
        {
            try
            {
                if (System.IO.File.Exists(_filePath))
                {
                    var json = System.IO.File.ReadAllText(_filePath);
                    var productos = JsonSerializer.Deserialize<List<ArticuloGolf>>(json);
                    if (productos != null && productos.Count > 0)
                    {
                        // Forzar la imagen fija en todos los productos cargados
                        foreach (var p in productos)
                        {
                            p.ImagenUrl = "https://images.unsplash.com/photo-1519864600265-abb23847ef2c?auto=format&fit=crop&w=400&q=80";
                        }
                        return productos;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error cargando productos_golf.json: {ex.Message}");
            }
            // Si no existe el archivo o está vacío, usar los productos por defecto
            return new List<ArticuloGolf>
            {
                new ArticuloGolf { Id = 1, Nombre = "Palo de golf Pro", Tipo = "Palo", Marca = "Callaway" },
                new ArticuloGolf { Id = 2, Nombre = "Bola Premium", Tipo = "Bola", Marca = "Titleist" },
                new ArticuloGolf { Id = 3, Nombre = "Guante Soft", Tipo = "Guante", Marca = "FootJoy" }
            };
        }

        private static void SaveProductos()
        {
            try
            {
                var json = JsonSerializer.Serialize(_productos, new JsonSerializerOptions { WriteIndented = true });
                System.IO.File.WriteAllText(_filePath, json);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error guardando productos_golf.json: {ex.Message}");
            }
        }

        [HttpGet("catalogo")]
        public IEnumerable<ArticuloGolf> Catalogo()
        {
            return _productos;
        }

        [HttpPost("agregar")]
        public ActionResult<ArticuloGolf> AgregarProducto([FromBody] ArticuloGolf nuevo)
        {
            Debug.WriteLine($"Llamada recibida en AgregarProducto: {nuevo?.Nombre} - {nuevo?.Tipo} - {nuevo?.Marca}");
            if (nuevo == null || string.IsNullOrWhiteSpace(nuevo.Nombre) || string.IsNullOrWhiteSpace(nuevo.Tipo) || string.IsNullOrWhiteSpace(nuevo.Marca))
                return BadRequest("Datos inválidos");
            nuevo.Id = _productos.Any() ? _productos.Max(c => c.Id) + 1 : 1;
            // Siempre usar la imagen fija
            nuevo.ImagenUrl = "https://images.unsplash.com/photo-1519864600265-abb23847ef2c?auto=format&fit=crop&w=400&q=80";
            _productos.Add(nuevo);
            SaveProductos();
            return CreatedAtAction(nameof(Catalogo), new { id = nuevo.Id }, nuevo);
        }

        [HttpDelete("eliminar/{id}")]
        public IActionResult EliminarProducto(int id)
        {
            var prod = _productos.FirstOrDefault(c => c.Id == id);
            if (prod == null)
                return NotFound();
            _productos.Remove(prod);
            SaveProductos();
            return NoContent();
        }

        [HttpGet("tipos")]
        public ActionResult<IEnumerable<string>> ListarTipos()
        {
            var tipos = _productos.Select(c => c.Tipo).Distinct().ToList();
            return Ok(tipos);
        }
    }
}
