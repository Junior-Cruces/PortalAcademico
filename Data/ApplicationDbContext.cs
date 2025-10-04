using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PortalAcademico.Models; // ← Nombre del proyecto + .Models

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Curso> Cursos { get; set; } = default!;
    public DbSet<Matricula> Matriculas { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Restricción: un usuario no puede matricularse dos veces en el mismo curso
        builder.Entity<Matricula>()
            .HasIndex(m => new { m.UsuarioId, m.CursoId })
            .IsUnique();

        // Datos iniciales
        builder.Entity<Curso>().HasData(
            new Curso { Id = 1, Codigo = "PROG1", Nombre = "Programación I", Creditos = 4, CupoMaximo = 30, HorarioInicio = new TimeSpan(8, 0, 0), HorarioFin = new TimeSpan(10, 0, 0), Activo = true },
            new Curso { Id = 2, Codigo = "MATH1", Nombre = "Matemáticas I", Creditos = 3, CupoMaximo = 25, HorarioInicio = new TimeSpan(10, 0, 0), HorarioFin = new TimeSpan(12, 0, 0), Activo = true },
            new Curso { Id = 3, Codigo = "DB1", Nombre = "Bases de Datos", Creditos = 3, CupoMaximo = 20, HorarioInicio = new TimeSpan(14, 0, 0), HorarioFin = new TimeSpan(16, 0, 0), Activo = true }
        );
    }
}