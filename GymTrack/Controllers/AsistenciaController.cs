using Microsoft.AspNetCore.Mvc;
using GymTrack.Data;
using GymTrack.Models;

namespace GymTrack.Controllers
{
    public class AsistenciaController : Controller
    {
        private readonly GymContext _context;

        public AsistenciaController(GymContext context)
        {
            _context = context;
        }

        // LISTAR LAS ASISTENCIAS DEL CLIENTE QUE INICIÓ SESIÓN
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("Usuario") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            //SOLO EL CLIENTE MARCA ASISTENCIA
            if (HttpContext.Session.GetString("Rol") != "Cliente")
            {
                return RedirectToAction("Index", "Home");
            }
            int usuarioId = (int)HttpContext.Session.GetInt32("Id");
            var asistencias = _context.Asistencias
                .Where(a => a.UsuarioId == usuarioId)
                .OrderByDescending(a => a.Fecha)
                .ToList();
            return View(asistencias);
        }

        // MOSTRAR EL FORMULARIO PARA MARCAR ASISTENCIA
        public IActionResult Create()
        {
            if (HttpContext.Session.GetString("Usuario") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            if (HttpContext.Session.GetString("Rol") != "Cliente")
            {
                return RedirectToAction("Index", "Home");
            }
            // El formulario empieza con la fecha de hoy y la casilla marcada
            return View(new Asistencia { Fecha = DateTime.Today, Asistio = true });
        }

        // GUARDAR LA ASISTENCIA EN LA BD
        [HttpPost]
        public IActionResult Create(Asistencia asistencia)
        {
            if (HttpContext.Session.GetString("Usuario") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            if (HttpContext.Session.GetString("Rol") != "Cliente")
            {
                return RedirectToAction("Index", "Home");
            }
            asistencia.UsuarioId = (int)HttpContext.Session.GetInt32("Id");

            // No permitir dos registros de asistencia en la misma fecha
            if (_context.Asistencias.Any(a => a.UsuarioId == asistencia.UsuarioId && a.Fecha == asistencia.Fecha))
            {
                ViewBag.Error = "Ya marcaste tu asistencia para esa fecha. Puedes editarla desde la lista.";
                return View(asistencia);
            }

            if (ModelState.IsValid)
            {
                _context.Asistencias.Add(asistencia);
                _context.SaveChanges();
                TempData["Mensaje"] = "Asistencia registrada correctamente.";
                return RedirectToAction("Index");
            }
            return View(asistencia);
        }

        // MOSTRAR EL FORMULARIO PARA EDITAR UNA ASISTENCIA
        public IActionResult Edit(int id)
        {
            if (HttpContext.Session.GetString("Usuario") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            if (HttpContext.Session.GetString("Rol") != "Cliente")
            {
                return RedirectToAction("Index", "Home");
            }
            var asistencia = _context.Asistencias.Find(id);
            if (asistencia == null)
            {
                return NotFound();
            }
            // Solo puede editar sus propias asistencias
            if (asistencia.UsuarioId != HttpContext.Session.GetInt32("Id"))
            {
                TempData["Error"] = "No puedes modificar registros de otro usuario.";
                return RedirectToAction("Index");
            }
            return View(asistencia);
        }

        // ACTUALIZAR LA ASISTENCIA EN LA BD
        [HttpPost]
        public IActionResult Edit(Asistencia asistencia)
        {
            if (HttpContext.Session.GetString("Usuario") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            if (HttpContext.Session.GetString("Rol") != "Cliente")
            {
                return RedirectToAction("Index", "Home");
            }
            int usuarioId = (int)HttpContext.Session.GetInt32("Id");
            // Verificar que la asistencia que se quiere modificar sea del usuario de la sesión
            if (!_context.Asistencias.Any(a => a.Id == asistencia.Id && a.UsuarioId == usuarioId))
            {
                TempData["Error"] = "No puedes modificar registros de otro usuario.";
                return RedirectToAction("Index");
            }
            asistencia.UsuarioId = usuarioId;

            // Si cambió la fecha, que no choque con otra asistencia suya (sin contarse a sí misma)
            if (_context.Asistencias.Any(a => a.UsuarioId == usuarioId && a.Fecha == asistencia.Fecha && a.Id != asistencia.Id))
            {
                ViewBag.Error = "Ya tienes otra asistencia registrada en esa fecha.";
                return View(asistencia);
            }

            if (ModelState.IsValid)
            {
                _context.Asistencias.Update(asistencia);
                _context.SaveChanges();
                TempData["Mensaje"] = "Asistencia actualizada correctamente.";
                return RedirectToAction("Index");
            }
            return View(asistencia);
        }

        // ELIMINAR UNA ASISTENCIA
        public IActionResult Delete(int id)
        {
            if (HttpContext.Session.GetString("Usuario") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            if (HttpContext.Session.GetString("Rol") != "Cliente")
            {
                return RedirectToAction("Index", "Home");
            }
            var asistencia = _context.Asistencias.Find(id);
            if (asistencia == null)
            {
                return NotFound();
            }
            if (asistencia.UsuarioId != HttpContext.Session.GetInt32("Id"))
            {
                TempData["Error"] = "No puedes eliminar registros de otro usuario.";
                return RedirectToAction("Index");
            }
            _context.Asistencias.Remove(asistencia);
            _context.SaveChanges();
            TempData["Mensaje"] = "Asistencia eliminada correctamente.";
            return RedirectToAction("Index");
        }
    }
}