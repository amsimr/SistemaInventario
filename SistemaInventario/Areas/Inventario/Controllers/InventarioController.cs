using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaInventario.AccesoDatos.Data;
using SistemaInventario.Utilidades;

namespace SistemaInventario.Areas.Inventario.Controllers
{
    [Area("Inventario")]
    [Authorize(Roles =DS.Role_Admin+","+DS.Role_Inventario)]
    public class InventarioController : Controller
    {
        private readonly ApplicationDbContext _db;

        public InventarioController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            return View();
        }




        #region API

        [HttpGet]
        public IActionResult ObtenerTodos()
        {
            var todos = _db.BodegaProducto.Include(b => b.Bodega).Include(p => p.Producto).ToList();
            return Json(new {data = todos});
        }

        #endregion



    }
}
