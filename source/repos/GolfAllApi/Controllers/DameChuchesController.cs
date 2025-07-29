using Microsoft.AspNetCore.Mvc;
using GolfAllApi.Models;
using System.Collections.Generic;
using System.Linq;

namespace GolfAllApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductosGolfController : ControllerBase
    {
        // Lista estática para simular almacenamiento en memoria
        private static List<ProductoGolf> _productos = new List<ProductoGolf>
        {
            new ProductoGolf { Id = 1, Nombre = "Palo de golf Pro", Tipo = "Palo", Marca = "Callaway" },
            new ProductoGolf { Id = 2, Nombre = "Bola Premium", Tipo = "Bola", Marca = "Titleist" },
            new ProductoGolf { Id = 3, Nombre = "Guante Soft", Tipo = "Guante", Marca = "FootJoy" }
        };

        [HttpGet("catalogo")]
        public IEnumerable<ProductoGolf> Catalogo()
        {
            return _productos;
        }

        [HttpPost("agregar")]
        public ActionResult<ProductoGolf> AgregarProducto([FromBody] ProductoGolf nuevo)
        {
            if (nuevo == null || string.IsNullOrWhiteSpace(nuevo.Nombre) || string.IsNullOrWhiteSpace(nuevo.Tipo) || string.IsNullOrWhiteSpace(nuevo.Marca))
                return BadRequest("Datos inválidos");
            nuevo.Id = _productos.Any() ? _productos.Max(c => c.Id) + 1 : 1;
            _productos.Add(nuevo);
            return CreatedAtAction(nameof(Catalogo), new { id = nuevo.Id }, nuevo);
        }

        [HttpDelete("eliminar/{id}")]
        public IActionResult EliminarProducto(int id)
        {
            var prod = _productos.FirstOrDefault(c => c.Id == id);
            if (prod == null)
                return NotFound();
            _productos.Remove(prod);
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