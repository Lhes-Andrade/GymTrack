using GymTrack.Data;
using GymTrack.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymTrack.Controllers
{
    public class EjercicioController : Controller
    {
        private readonly GymContext _context;

        public EjercicioController(GymContext context)
        {
            _context = context;
        }

        // LISTAR LOS EJERCICIOS (ADMIN Y CLIENTE PUEDEN VER)
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("Usuario") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            var ejercicios = _context.Ejercicios
                .OrderBy(e => e.Nombre)
                .ToList();
            return View(ejercicios);
        }

        // MOSTRAR EL FORMULARIO PARA CREAR UN EJERCICIO
        public IActionResult Create()
        {
            if (HttpContext.Session.GetString("Usuario") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            //SOLO EL ADMIN PUEDE CREAR EJERCICIOS
            if (HttpContext.Session.GetString("Rol") != "Administrador")
            {
                TempData["Error"] = "No tienes permiso para realizar esta acción.";
                return RedirectToAction("Index");
            }
            return View();
        }

        // GUARDAR EL NUEVO EJERCICIO EN LA BD
        [HttpPost]
        public IActionResult Create(Ejercicio ejercicio)
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
                _context.Ejercicios.Add(ejercicio);
                _context.SaveChanges();
                TempData["Mensaje"] = "Ejercicio creado correctamente.";
                return RedirectToAction("Index");
            }
            return View(ejercicio);
        }

        // MOSTRAR EL FORMULARIO PARA EDITAR UN EJERCICIO
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
            var ejercicio = _context.Ejercicios.Find(id);
            if (ejercicio == null)
            {
                return NotFound();
            }
            return View(ejercicio);
        }

        // ACTUALIZAR EL EJERCICIO EN LA BD
        [HttpPost]
        public IActionResult Edit(Ejercicio ejercicio)
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
                _context.Ejercicios.Update(ejercicio);
                _context.SaveChanges();
                TempData["Mensaje"] = "Ejercicio actualizado correctamente.";
                return RedirectToAction("Index");
            }
            return View(ejercicio);
        }

        // ELIMINAR UN EJERCICIO DE LA BD
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
            var ejercicio = _context.Ejercicios.Find(id);
            if (ejercicio == null)
            {
                return NotFound();
            }
            // No permitir eliminar si el ejercicio ya se usa en entrenamientos o rutinas
            if (_context.RegistrosEntrenamiento.Any(r => r.EjercicioId == id) ||
                _context.RutinaEjercicios.Any(r => r.EjercicioId == id))
            {
                TempData["Error"] = "No se puede eliminar el ejercicio porque está en uso en entrenamientos o rutinas.";
                return RedirectToAction("Index");
            }
            _context.Ejercicios.Remove(ejercicio);
            _context.SaveChanges();
            TempData["Mensaje"] = "Ejercicio eliminado correctamente.";
            return RedirectToAction("Index");
        }
    }
}