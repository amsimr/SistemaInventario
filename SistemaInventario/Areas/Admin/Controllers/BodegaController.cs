﻿using Microsoft.AspNetCore.Mvc;
using SistemaInventario.AccesoDatos.Repositorio.IRepositorio;
using SistemaInventario.Modelos;

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


        // Vista Index

        public IActionResult Index()
        {
            return View();
        }



        // Metodo Upsert

        public IActionResult Upsert(int? id)
        {
            Bodega bodega = new Bodega();
            if (id == null)
            {
                // Esto es para crear un nuevo registro
                return View(bodega);
            }
            // Esto es para actualizar
            bodega = _unidadTrabajo.Bodega.Obtener(id.GetValueOrDefault());
            if (bodega == null)
            {
                return NotFound();
            }

            return View(bodega);
        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Bodega bodega)
        {
            if (ModelState.IsValid)
            {
                if (bodega.Id == 0)
                {
                    _unidadTrabajo.Bodega.Agregar(bodega);
                }
                else
                {
                    _unidadTrabajo.Bodega.Actualizar(bodega);
                }
                _unidadTrabajo.Guardar();
                return RedirectToAction(nameof(Index));
            }
            return View(bodega);
        }









        #region API
        [HttpGet]
        public IActionResult ObtenerTodos()
        {
            var todos = _unidadTrabajo.Bodega.ObtenerTodos();
            return Json(new { data = todos });
        }


        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var bodegaDb = _unidadTrabajo.Bodega.Obtener(id);
            if (bodegaDb == null)
            {
                return Json(new { success = false, message = "Error al borrar" });
            }
            _unidadTrabajo.Bodega.Remover(bodegaDb);
            _unidadTrabajo.Guardar();
            return Json(new { success = true, message = "Borrado exitosamente" });
        }

        #endregion

    }
}
