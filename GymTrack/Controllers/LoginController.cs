using Microsoft.AspNetCore.Mvc;
using GymTrack.Data;
using GymTrack.Models;
using GymTrack.Helpers;

namespace GymTrack.Controllers
{
    public class LoginController : Controller
    {
        private readonly GymContext _context;

        public LoginController(GymContext context)
        {
            _context = context;
        }

        // MOSTRAR EL FORMULARIO DE LOGIN
        public IActionResult Index()
        {
            return View();
        }

        // VALIDAR CORREO Y CLAVE
        [HttpPost]
        public IActionResult Index(string correo, string clave)
        {
            string claveHash = HashHelper.ObtenerHash(clave);
            var usuario = _context.Usuarios
                .FirstOrDefault(u => u.Correo == correo && u.Clave == claveHash);

            if (usuario != null)
            {
                // Guardamos en Session el nombre, el rol y el Id del usuario
                HttpContext.Session.SetString("Usuario", usuario.Nombre);
                HttpContext.Session.SetString("Rol", usuario.Rol);
                HttpContext.Session.SetInt32("Id", usuario.Id);
                return RedirectToAction("Index", "Home");
            }
            ViewBag.Error = "Correo o clave incorrectas";
            return View();
        }

        // MOSTRAR EL FORMULARIO DE REGISTRO
        public IActionResult Registro()
        {
            return View();
        }

        // GUARDAR EL NUEVO CLIENTE
        [HttpPost]
        public IActionResult Registro(Usuario usuario)
        {
            // El rol lo asigna el controlador, no el formulario
            usuario.Rol = "Cliente";
            ModelState.Remove("Rol");

            // Validar que el correo no esté registrado
            if (_context.Usuarios.Any(u => u.Correo == usuario.Correo))
            {
                ViewBag.Error = "Ya existe una cuenta con ese correo";
                return View(usuario);
            }

            if (ModelState.IsValid)
            {
                usuario.Clave = HashHelper.ObtenerHash(usuario.Clave);
                _context.Usuarios.Add(usuario);
                _context.SaveChanges();
                TempData["Mensaje"] = "Cuenta creada. Ya puedes iniciar sesión.";
                return RedirectToAction("Index");
            }
            return View(usuario);
        }

        // CERRAR SESIÓN
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            TempData["Mensaje"] = "Sesión Cerrada Correctamente";
            return RedirectToAction("Index", "Login");
        }
    }
}