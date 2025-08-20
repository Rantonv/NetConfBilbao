using GolfAllApi.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text.Json;

namespace GolfAllApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductosGolfController : ControllerBase
    {
        private readonly ILogger<ProductosGolfController> _logger;
        private static readonly string _filePath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "productos_golf.json");
        private static List<ArticuloGolf> _productos = LoadProductos();

        public ProductosGolfController(ILogger<ProductosGolfController> logger)
        {
            _logger = logger;
        }

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
                new ArticuloGolf { Id = 1, Nombre = "Palo de golf Pro", Tipo = "Palo", Marca = "Callaway", ImagenUrl = "https://images.unsplash.com/photo-1562204320-c7f5f2a04156?q=80&w=1170&auto=format&fit=crop" },
                new ArticuloGolf { Id = 2, Nombre = "Bola Premium", Tipo = "Bola", Marca = "Titleist", ImagenUrl = "https://images.unsplash.com/photo-1703293024102-44224053a305?q=80&w=1261&auto=format&fit=crop"},
                new ArticuloGolf { Id = 3, Nombre = "Guante Soft", Tipo = "Guante", Marca = "FootJoy", ImagenUrl = "https://images.unsplash.com/photo-1689323473750-75520243edcb?q=80&w=1332&auto=format&fit=crop"}
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
            _logger.LogInformation("Llamada a Catalogo");
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

        [HttpGet("catalogo/tipo/{tipo}")]
        public ActionResult<IEnumerable<ArticuloGolf>> CatalogoPorTipo(string tipo)
        {
            if (string.IsNullOrWhiteSpace(tipo))
                return BadRequest("Tipo inválido");
            var productosFiltrados = _productos.Where(p => p.Tipo.Equals(tipo, StringComparison.OrdinalIgnoreCase)).ToList();
            return Ok(productosFiltrados);
        }
    }
}