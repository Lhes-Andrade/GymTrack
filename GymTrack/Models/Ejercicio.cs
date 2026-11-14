using System.ComponentModel.DataAnnotations;

namespace GymTrack.Models
{
    public class Ejercicio
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100)]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El grupo muscular es obligatorio")]
        public string GrupoMuscular { get; set; }

        public string? Equipo { get; set; }

        public string? Descripcion { get; set; }
    }
}