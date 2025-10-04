using System.ComponentModel.DataAnnotations;

namespace PortalAcademico.Models;

public class Curso
{
    public int Id { get; set; }

    [Required]
    [StringLength(10)]
    public string Codigo { get; set; } = string.Empty;

    [Required]
    public string Nombre { get; set; } = string.Empty;

    [Range(1, 10, ErrorMessage = "Los créditos deben estar entre 1 y 10.")]
    public int Creditos { get; set; }

    [Range(1, 50, ErrorMessage = "El cupo máximo debe estar entre 1 y 50.")]
    public int CupoMaximo { get; set; }

    public TimeOnly HorarioInicio { get; set; }
    public TimeOnly HorarioFin { get; set; }

    public bool Activo { get; set; } = true;
}