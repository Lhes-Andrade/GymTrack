using System.ComponentModel.DataAnnotations;

namespace GymTrack.Models
{
    public class Asistencia
    {
        public int Id { get; set; }

        public int UsuarioId { get; set; }
        public Usuario? Usuario { get; set; }

        [Required(ErrorMessage = "La fecha es obligatoria")]
        public DateTime Fecha { get; set; }

        public bool Asistio { get; set; }

        public string? Nota { get; set; }
    }
}