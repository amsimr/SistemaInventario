using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SistemaInventario.AccesoDatos.Repositorio.IRepositorio;
using SistemaInventario.Modelos;
using SistemaInventario.Modelos.ViewModels;

namespace SistemaInventario.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductoController : Controller
    {
        private readonly IUnidadTrabajo _unidadTrabajo;
        private readonly IWebHostEnvironment _hostEnvironment;

        public ProductoController(IUnidadTrabajo unidadTrabajo, IWebHostEnvironment hostEnvironment)
        {
            _unidadTrabajo = unidadTrabajo;
            _hostEnvironment = hostEnvironment;
        }


        // Vista Index

        public IActionResult Index()
        {
            return View();
        }



        // Metodo Upsert

        public IActionResult Upsert(int? id)
        {
            ProductoVM productoVM = new ProductoVM() {

               Producto = new Producto(),
               CategoriaLista = _unidadTrabajo.Categoria.ObtenerTodos().Select(c => new SelectListItem {
                   Text = c.Nombre,
                   Value = c.Id.ToString()
               }),
               MarcaLista = _unidadTrabajo.Marca.ObtenerTodos().Select(m => new SelectListItem { 
                
                   Text = m.Nombre,
                   Value = m.Id.ToString()
               })

            };



            if (id == null)
            {
                // Esto es para crear un nuevo registro
                return View(productoVM);
            }
            // Esto es para actualizar
            productoVM.Producto = _unidadTrabajo.Producto.Obtener(id.GetValueOrDefault());
            if (productoVM.Producto == null)
            {
                return NotFound();
            }

            return View(productoVM);
        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Producto producto)
        {
            if (ModelState.IsValid)
            {
                if (producto.Id == 0)
                {
                    _unidadTrabajo.Producto.Agregar(producto);
                }
                else
                {
                    _unidadTrabajo.Producto.Actualizar(producto);
                }
                _unidadTrabajo.Guardar();
                return RedirectToAction(nameof(Index));
            }
            return View(producto);
        }









        #region API
        [HttpGet]
        public IActionResult ObtenerTodos()
        {
            var todos = _unidadTrabajo.Producto.ObtenerTodos(incluirPropiedades: "Categoria,Marca");
            return Json(new { data = todos });
        }


        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var productoDb = _unidadTrabajo.Producto.Obtener(id);
            if (productoDb == null)
            {
                return Json(new { success = false, message = "Error al borrar" });
            }
            _unidadTrabajo.Producto.Remover(productoDb);
            _unidadTrabajo.Guardar();
            return Json(new { success = true, message = "Borrado exitosamente" });
        }

        #endregion

    }
}
