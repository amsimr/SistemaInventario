using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
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
        public IActionResult Upsert(ProductoVM productoVM)
        {
            if (ModelState.IsValid)
            {

                // Cargar imagenes
                string webRootPath = _hostEnvironment.WebRootPath;
                var files = HttpContext.Request.Form.Files;
                if (files.Count > 0)
                {
                    string filename = Guid.NewGuid().ToString();
                    var uploads = Path.Combine(webRootPath, @"imagenes\productos");
                    var extension = Path.GetExtension(files[0].FileName);
                    if (productoVM.Producto.ImagenUrl != null)
                    {
                        // Esto es para editar, necesitaremos borrar la imagen anterior
                        var imagenPath = Path.Combine(webRootPath, productoVM.Producto.ImagenUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(imagenPath))
                        {
                            System.IO.File.Delete(imagenPath);
                        }
                    }


                    using (var filesStreams = new FileStream(Path.Combine(uploads, filename + extension), FileMode.Create))
                    {
                        files[0].CopyTo(filesStreams);
                    }
                    productoVM.Producto.ImagenUrl = @"\imagenes\productos\" + filename + extension;
                }
                else
                {
                    // Si en el update el usuario no cambia la imagen
                    if (productoVM.Producto.Id != 0)
                    {
                        Producto productoDb = _unidadTrabajo.Producto.Obtener(productoVM.Producto.Id);
                        productoVM.Producto.ImagenUrl = productoDb.ImagenUrl;
                    }

                }



                if (productoVM.Producto.Id == 0)
                {
                    _unidadTrabajo.Producto.Agregar(productoVM.Producto);
                }
                else
                {
                    _unidadTrabajo.Producto.Actualizar(productoVM.Producto);
                }
                _unidadTrabajo.Guardar();
                return RedirectToAction(nameof(Index));
            }
            else
            {
                productoVM.CategoriaLista = _unidadTrabajo.Categoria.ObtenerTodos().Select(c => new SelectListItem
                {
                    Text = c.Nombre,
                    Value = c.Id.ToString()
                });
                productoVM.MarcaLista = _unidadTrabajo.Marca.ObtenerTodos().Select(m => new SelectListItem
                {

                    Text = m.Nombre,
                    Value = m.Id.ToString()
                });


                if (productoVM.Producto.Id!=0)
                {
                    productoVM.Producto = _unidadTrabajo.Producto.Obtener(productoVM.Producto.Id);
                }


            }
            return View(productoVM.Producto);
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


            // Eliminar la imagen relacionada al producto
            string webRootPath = _hostEnvironment.WebRootPath;
            var imagenPath = Path.Combine(webRootPath, productoDb.ImagenUrl.TrimStart('\\'));
            if (System.IO.File.Exists(imagenPath))
            {
                System.IO.File.Delete(imagenPath);
            }


            _unidadTrabajo.Producto.Remover(productoDb);
            _unidadTrabajo.Guardar();
            return Json(new { success = true, message = "Borrado exitosamente" });
        }

        #endregion

    }
}
