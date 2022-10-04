using Microsoft.AspNetCore.Mvc;
using SistemaInventario.AccesoDatos.Repositorio.IRepositorio;

namespace SistemaInventario.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BodegaController : Controller
    {
        private readonly IUnidadTrabajo _unidadTrabajo;

        public BodegaController(IUnidadTrabajo unidadTrabajo)
        {
            _unidadTrabajo = unidadTrabajo;
        }



        public IActionResult Index()
        {
            return View();
        }


        #region API
        [HttpGet]
        public IActionResult ObtenerTodos()
        {
            var todos = _unidadTrabajo.Bodega.ObtenerTodos();
            return Json(new { data = todos });
        }

        #endregion

    }
}
