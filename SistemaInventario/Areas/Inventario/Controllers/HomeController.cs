using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaInventario.AccesoDatos.Data;
using SistemaInventario.AccesoDatos.Repositorio.IRepositorio;
using SistemaInventario.Modelos;
using SistemaInventario.Modelos.ViewModels;
using System.Diagnostics;

namespace SistemaInventario.Areas.Inventario.Controllers
{
    [Area("Inventario")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnidadTrabajo _unidadTrabajo;
        private readonly ApplicationDbContext _db;

        [BindProperty]
        public CarroComprasViewModel CarroComprasVM { get; set; }

        public HomeController(ILogger<HomeController> logger, IUnidadTrabajo unidadTrabajo, ApplicationDbContext db)
        {
            _logger = logger;
            _unidadTrabajo = unidadTrabajo;
            _db = db;
        }

        public IActionResult Index()
        {
            IEnumerable<Producto> productoLista = _unidadTrabajo.Producto.ObtenerTodos(incluirPropiedades: "Categoria,Marca");
            return View(productoLista);
        }

        public IActionResult Detalle(int id)
        {
            CarroComprasVM = new CarroComprasViewModel();
            CarroComprasVM.Compania = _db.Compania.FirstOrDefault();
            CarroComprasVM.BodegaProducto = _db.BodegaProducto.Include(p => p.Producto).Include(p => p.Producto.Categoria).Include(p => p.Producto.Marca)
                                                 .FirstOrDefault(b => b.ProductoId == id && b.BodegaId == CarroComprasVM.Compania.BodegaVentaId);

            if (CarroComprasVM.BodegaProducto == null)
            {
                return RedirectToAction("Index");
            }
            else
            {
                CarroComprasVM.CarroCompras = new CarroCompras()
                {
                    Producto = CarroComprasVM.BodegaProducto.Producto,
                    ProductoId = CarroComprasVM.BodegaProducto.ProductoId
                };
                return View(CarroComprasVM);
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}