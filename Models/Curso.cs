using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PortalAcademico.Models
{
    public class Curso
    {
        public int Id { get; set; }

        [Required]
        [StringLength(10)]
        public string Codigo { get; set; } = string.Empty;

        [Required]
        public string Nombre { get; set; } = string.Empty;

        [Range(1, 10)]
        public int Creditos { get; set; }

        [Range(1, 100)]
        public int CupoMaximo { get; set; }

        public TimeSpan HorarioInicio { get; set; }
        public TimeSpan HorarioFin { get; set; }

        public bool Activo { get; set; } = true;

        public bool EsHorarioValido() => HorarioInicio < HorarioFin;
    }
}