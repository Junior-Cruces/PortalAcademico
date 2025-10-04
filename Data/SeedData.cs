using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PortalAcademico.Models;

namespace PortalAcademico.Data;

public static class SeedData
{
    public static async Task Initialize(IServiceProvider serviceProvider)
    {
        var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
        var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        // Asegurar que la BD exista (solo para desarrollo)
        context.Database.EnsureCreated();

        // Crear rol Coordinador
        if (!await roleManager.RoleExistsAsync("Coordinador"))
        {
            await roleManager.CreateAsync(new IdentityRole("Coordinador"));
        }

        // Crear usuario Coordinador
        const string email = "coordinador@universidad.edu";
        if (await userManager.FindByEmailAsync(email) == null)
        {
            var user = new IdentityUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true
            };
            await userManager.CreateAsync(user, "Coordinador123!");
            await userManager.AddToRoleAsync(user, "Coordinador");
        }

        // Sembrar cursos (solo si no existen)
        if (context.Cursos.Any()) return;

        var cursos = new Curso[]
        {
            new() { Codigo = "MAT101", Nombre = "Cálculo I", Creditos = 4, CupoMaximo = 30, HorarioInicio = new TimeOnly(8, 0), HorarioFin = new TimeOnly(10, 0), Activo = true },
            new() { Codigo = "FIS101", Nombre = "Física I", Creditos = 3, CupoMaximo = 25, HorarioInicio = new TimeOnly(10, 0), HorarioFin = new TimeOnly(12, 0), Activo = true },
            new() { Codigo = "PROG1", Nombre = "Programación I", Creditos = 5, CupoMaximo = 20, HorarioInicio = new TimeOnly(14, 0), HorarioFin = new TimeOnly(16, 0), Activo = true }
        };

        context.Cursos.AddRange(cursos);
        await context.SaveChangesAsync();
    }
}