using System.ComponentModel.DataAnnotations;

namespace GymTrack.Models
{
    public class Rutina
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100)]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El objetivo es obligatorio")]
        public string Objetivo { get; set; }

        [Required(ErrorMessage = "El nivel es obligatorio")]
        public string Nivel { get; set; }

        public string? Descripcion { get; set; }
    }
}