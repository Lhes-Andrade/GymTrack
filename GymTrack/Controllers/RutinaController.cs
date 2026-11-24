using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GymTrack.Data;
using GymTrack.Models;

namespace GymTrack.Controllers
{
    public class RutinaController : Controller
    {
        private readonly GymContext _context;

        public RutinaController(GymContext context)
        {
            _context = context;
        }

        // LISTAR LAS RUTINAS (ADMIN Y CLIENTE PUEDEN VER)
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("Usuario") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            var rutinas = _context.Rutinas.ToList();
            return View(rutinas);
        }

        // VER UNA RUTINA CON SUS EJERCICIOS (ADMIN Y CLIENTE PUEDEN VER)
        public IActionResult Detalle(int id)
        {
            if (HttpContext.Session.GetString("Usuario") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            var rutina = _context.Rutinas.Find(id);
            if (rutina == null)
            {
                return NotFound();
            }
            // Ejercicios que pertenecen a esta rutina, trayendo también el nombre del ejercicio
            ViewBag.EjerciciosRutina = _context.RutinaEjercicios
                .Include(r => r.Ejercicio)
                .Where(r => r.RutinaId == id)
                .ToList();
            // Todos los ejercicios, para la lista desplegable del administrador
            ViewBag.Ejercicios = _context.Ejercicios.ToList();
            return View(rutina);
        }

        // MOSTRAR EL FORMULARIO PARA CREAR UNA RUTINA
        public IActionResult Create()
        {
            if (HttpContext.Session.GetString("Usuario") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            //SOLO EL ADMIN PUEDE CREAR RUTINAS
            if (HttpContext.Session.GetString("Rol") != "Administrador")
            {
                TempData["Error"] = "No tienes permiso para realizar esta acción.";
                return RedirectToAction("Index");
            }
            return View("Index");
        }

        // GUARDAR LA NUEVA RUTINA EN LA BD
        [HttpPost]
        public IActionResult Create(Rutina rutina)
        {
            if (HttpContext.Session.GetString("Usuario") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            if (HttpContext.Session.GetString("Rol") != "Administrador")
            {
                TempData["Error"] = "No tienes permiso para realizar esta acción.";
                return RedirectToAction("Index");
            }
            if (ModelState.IsValid)
            {
                _context.Rutinas.Add(rutina);
                _context.SaveChanges();
                TempData["Mensaje"] = "Rutina creada. Ahora agrégale ejercicios.";
                return RedirectToAction("Detalle", new { id = rutina.Id });
            }
            return View(rutina);
        }

        // MOSTRAR EL FORMULARIO PARA EDITAR UNA RUTINA
        public IActionResult Edit(int id)
        {
            if (HttpContext.Session.GetString("Usuario") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            if (HttpContext.Session.GetString("Rol") != "Administrador")
            {
                TempData["Error"] = "No tienes permiso para realizar esta acción.";
                return RedirectToAction("Index");
            }
            var rutina = _context.Rutinas.Find(id);
            if (rutina == null)
            {
                return NotFound();
            }
            return View(rutina);
        }

        // ACTUALIZAR LA RUTINA EN LA BD
        [HttpPost]
        public IActionResult Edit(Rutina rutina)
        {
            if (HttpContext.Session.GetString("Usuario") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            if (HttpContext.Session.GetString("Rol") != "Administrador")
            {
                TempData["Error"] = "No tienes permiso para realizar esta acción.";
                return RedirectToAction("Index");
            }
            if (ModelState.IsValid)
            {
                _context.Rutinas.Update(rutina);
                _context.SaveChanges();
                TempData["Mensaje"] = "Rutina actualizada correctamente.";
                return RedirectToAction("Index");
            }
            return View(rutina);
        }

        // ELIMINAR UNA RUTINA (SE BORRAN TAMBIÉN SUS EJERCICIOS ASIGNADOS)
        public IActionResult Delete(int id)
        {
            if (HttpContext.Session.GetString("Usuario") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            if (HttpContext.Session.GetString("Rol") != "Administrador")
            {
                TempData["Error"] = "No tienes permiso para realizar esta acción.";
                return RedirectToAction("Index");
            }
            var rutina = _context.Rutinas.Find(id);
            if (rutina == null)
            {
                return NotFound();
            }
            _context.Rutinas.Remove(rutina);
            _context.SaveChanges();
            TempData["Mensaje"] = "Rutina eliminada correctamente.";
            return RedirectToAction("Index");
        }

        // AGREGAR UN EJERCICIO A LA RUTINA
        [HttpPost]
        public IActionResult AgregarEjercicio(RutinaEjercicio rutinaEjercicio)
        {
            if (HttpContext.Session.GetString("Usuario") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            if (HttpContext.Session.GetString("Rol") != "Administrador")
            {
                TempData["Error"] = "No tienes permiso para realizar esta acción.";
                return RedirectToAction("Index");
            }
            if (ModelState.IsValid)
            {
                _context.RutinaEjercicios.Add(rutinaEjercicio);
                _context.SaveChanges();
                TempData["Mensaje"] = "Ejercicio agregado a la rutina.";
            }
            else
            {
                TempData["Error"] = "Revisa los datos: selecciona un ejercicio y valores válidos.";
            }
            return RedirectToAction("Detalle", new { id = rutinaEjercicio.RutinaId });
        }

        // QUITAR UN EJERCICIO DE LA RUTINA
        public IActionResult QuitarEjercicio(int id)
        {
            if (HttpContext.Session.GetString("Usuario") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            if (HttpContext.Session.GetString("Rol") != "Administrador")
            {
                TempData["Error"] = "No tienes permiso para realizar esta acción.";
                return RedirectToAction("Index");
            }
            var rutinaEjercicio = _context.RutinaEjercicios.Find(id);
            if (rutinaEjercicio == null)
            {
                return NotFound();
            }
            int rutinaId = rutinaEjercicio.RutinaId;
            _context.RutinaEjercicios.Remove(rutinaEjercicio);
            _context.SaveChanges();
            TempData["Mensaje"] = "Ejercicio quitado de la rutina.";
            return RedirectToAction("Detalle", new { id = rutinaId });
        }
    }
}