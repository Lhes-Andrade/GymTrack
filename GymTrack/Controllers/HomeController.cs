using GymTrack.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace GymTrack.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("Usuario") == null)
            {
                return RedirectToAction("Index", "Login");
            }


            if (HttpContext.Session.GetString("Rol") == "Cliente")
            {
                return RedirectToAction("Index", "Entrenamiento");
            }

            if (HttpContext.Session.GetString("Rol") == "Administrador")
            {
                return RedirectToAction("Index", "Ejercicio");
            }

            return View();
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
