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
