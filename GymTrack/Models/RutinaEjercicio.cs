using System.ComponentModel.DataAnnotations;

namespace GymTrack.Models
{
    public class RutinaEjercicio
    {
        public int Id { get; set; }

        public int RutinaId { get; set; }
        public Rutina? Rutina { get; set; }

        public int EjercicioId { get; set; }
        public Ejercicio? Ejercicio { get; set; }

        [Range(1, 20, ErrorMessage = "Las series deben estar entre 1 y 20")]
        public int Series { get; set; }

        [Range(1, 100, ErrorMessage = "Las repeticiones deben estar entre 1 y 100")]
        public int Repeticiones { get; set; }

        [Range(0, 600, ErrorMessage = "El descanso debe estar entre 0 y 600 segundos")]
        public int DescansoSegundos { get; set; }
    }
}