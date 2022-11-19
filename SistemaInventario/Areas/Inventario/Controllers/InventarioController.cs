using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SistemaInventario.AccesoDatos.Data;
using SistemaInventario.Modelos.ViewModels;
using SistemaInventario.Utilidades;

namespace SistemaInventario.Areas.Inventario.Controllers
{
    [Area("Inventario")]
    [Authorize(Roles =DS.Role_Admin+","+DS.Role_Inventario)]
    public class InventarioController : Controller
    {
        private readonly ApplicationDbContext _db;

        [BindProperty]
        public InventarioViewModel inventarioVM { get; set; }

        public InventarioController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            return View();
        }


        public IActionResult NuevoInventario(int? inventarioId)
        {
            inventarioVM = new InventarioViewModel();
            inventarioVM.BodegaLista = _db.Bodegas.ToList().Select(b => new SelectListItem
            {
                Text = b.Nombre,
                Value = b.Id.ToString()
            });
            inventarioVM.ProductoLista = _db.Productos.ToList().Select(p => new SelectListItem
            {
                Text = p.Descripcion,
                Value = p.Id.ToString()
            });


            if (inventarioId!=null)
            {
                inventarioVM.Inventario = _db.Inventario.SingleOrDefault(i => i.Id == inventarioId);
                inventarioVM.InventarioDetalles = _db.InventarioDetalle.Include(p => p.Producto).ToList();
            }

            return View(inventarioVM);

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
