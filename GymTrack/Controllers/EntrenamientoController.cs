using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GymTrack.Data;
using GymTrack.Models;

namespace GymTrack.Controllers
{
    public class EntrenamientoController : Controller
    {
        private readonly GymContext _context;

        public EntrenamientoController(GymContext context)
        {
            _context = context;
        }

        // LISTAR LOS ENTRENAMIENTOS DEL CLIENTE QUE INICIÓ SESIÓN
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("Usuario") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            //SOLO EL CLIENTE REGISTRA ENTRENAMIENTOS
            if (HttpContext.Session.GetString("Rol") != "Cliente")
            {
                return RedirectToAction("Index", "Home");
            }
            int usuarioId = (int)HttpContext.Session.GetInt32("Id");
            var registros = _context.RegistrosEntrenamiento
                .Include(r => r.Ejercicio)
                .Where(r => r.UsuarioId == usuarioId)
                .OrderByDescending(r => r.Fecha)
                .ToList();
            return View(registros);
        }

        // MOSTRAR EL FORMULARIO PARA REGISTRAR UN ENTRENAMIENTO
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
            ViewBag.Ejercicios = _context.Ejercicios.ToList();
            // La fecha del formulario empieza con el día de hoy
            return View(new RegistroEntrenamiento { Fecha = DateTime.Today });
        }

        // GUARDAR EL ENTRENAMIENTO EN LA BD
        [HttpPost]
        public IActionResult Create(RegistroEntrenamiento registro)
        {
            if (HttpContext.Session.GetString("Usuario") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            if (HttpContext.Session.GetString("Rol") != "Cliente")
            {
                return RedirectToAction("Index", "Home");
            }
            // El UsuarioId lo pone el controlador con el Id de la sesión, no el formulario
            registro.UsuarioId = (int)HttpContext.Session.GetInt32("Id");

            if (ModelState.IsValid)
            {
                _context.RegistrosEntrenamiento.Add(registro);
                _context.SaveChanges();
                TempData["Mensaje"] = "Entrenamiento registrado correctamente.";
                return RedirectToAction("Index");
            }
            // Si hay errores, volvemos a llenar la lista de ejercicios antes de mostrar la vista
            ViewBag.Ejercicios = _context.Ejercicios.ToList();
            return View(registro);
        }

        // MOSTRAR EL FORMULARIO PARA EDITAR UN ENTRENAMIENTO
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
            var registro = _context.RegistrosEntrenamiento.Find(id);
            if (registro == null)
            {
                return NotFound();
            }
            // Solo puede editar sus propios registros
            if (registro.UsuarioId != HttpContext.Session.GetInt32("Id"))
            {
                TempData["Error"] = "No puedes modificar registros de otro usuario.";
                return RedirectToAction("Index");
            }
            ViewBag.Ejercicios = _context.Ejercicios.ToList();
            return View(registro);
        }

        // ACTUALIZAR EL ENTRENAMIENTO EN LA BD
        [HttpPost]
        public IActionResult Edit(RegistroEntrenamiento registro)
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
            // Verificar que el registro que se quiere modificar sea del usuario de la sesión
            if (!_context.RegistrosEntrenamiento.Any(r => r.Id == registro.Id && r.UsuarioId == usuarioId))
            {
                TempData["Error"] = "No puedes modificar registros de otro usuario.";
                return RedirectToAction("Index");
            }
            registro.UsuarioId = usuarioId;

            if (ModelState.IsValid)
            {
                _context.RegistrosEntrenamiento.Update(registro);
                _context.SaveChanges();
                TempData["Mensaje"] = "Entrenamiento actualizado correctamente.";
                return RedirectToAction("Index");
            }
            ViewBag.Ejercicios = _context.Ejercicios.ToList();
            return View(registro);
        }

        // ELIMINAR UN ENTRENAMIENTO
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
            var registro = _context.RegistrosEntrenamiento.Find(id);
            if (registro == null)
            {
                return NotFound();
            }
            if (registro.UsuarioId != HttpContext.Session.GetInt32("Id"))
            {
                TempData["Error"] = "No puedes eliminar registros de otro usuario.";
                return RedirectToAction("Index");
            }
            _context.RegistrosEntrenamiento.Remove(registro);
            _context.SaveChanges();
            TempData["Mensaje"] = "Entrenamiento eliminado correctamente.";
            return RedirectToAction("Index");
        }
    }
}