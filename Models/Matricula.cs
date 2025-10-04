using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity; // ← Necesario para IdentityUser

namespace PortalAcademico.Models
{
    public class Matricula
    {
        public int Id { get; set; }

        public int CursoId { get; set; }
        [ForeignKey("CursoId")]
        public Curso Curso { get; set; } = null!;

        public string UsuarioId { get; set; } = null!;
        [ForeignKey("UsuarioId")]
        public IdentityUser Usuario { get; set; } = null!;

        public DateTime FechaRegistro { get; set; } = DateTime.Now;

        [Required]
        public string Estado { get; set; } = "Pendiente"; // Pendiente, Confirmada, Cancelada
    }
}