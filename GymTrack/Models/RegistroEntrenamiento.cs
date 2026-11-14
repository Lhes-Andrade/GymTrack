using System.ComponentModel.DataAnnotations;

namespace GymTrack.Models
{
    public class RegistroEntrenamiento
    {
        public int Id { get; set; }

        public int UsuarioId { get; set; }
        public Usuario? Usuario { get; set; }

        public int EjercicioId { get; set; }
        public Ejercicio? Ejercicio { get; set; }

        [Required(ErrorMessage = "La fecha es obligatoria")]
        public DateTime Fecha { get; set; }

        [Range(1, 20, ErrorMessage = "Las series deben estar entre 1 y 20")]
        public int Series { get; set; }

        [Range(1, 100, ErrorMessage = "Las repeticiones deben estar entre 1 y 100")]
        public int Repeticiones { get; set; }

        [Range(0, 500, ErrorMessage = "El peso debe estar entre 0 y 500 kg")]
        public double PesoKg { get; set; }

        public string? Notas { get; set; }
    }
}