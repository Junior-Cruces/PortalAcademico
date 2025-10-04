using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PortalAcademico.Models;

namespace PortalAcademico.Data
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            using var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>());

            // Cursos iniciales
            if (!context.Cursos.Any())
            {
                context.Cursos.AddRange(
                    new Curso
                    {
                        Id = 1,
                        Codigo = "PROG1",
                        Nombre = "Programación I",
                        Creditos = 4,
                        CupoMaximo = 30,
                        HorarioInicio = new TimeSpan(8, 0, 0),
                        HorarioFin = new TimeSpan(10, 0, 0),
                        Activo = true
                    },
                    new Curso
                    {
                        Id = 2,
                        Codigo = "MATH1",
                        Nombre = "Matemáticas I",
                        Creditos = 3,
                        CupoMaximo = 25,
                        HorarioInicio = new TimeSpan(10, 0, 0),
                        HorarioFin = new TimeSpan(12, 0, 0),
                        Activo = true
                    },
                    new Curso
                    {
                        Id = 3,
                        Codigo = "DB1",
                        Nombre = "Bases de Datos",
                        Creditos = 3,
                        CupoMaximo = 20,
                        HorarioInicio = new TimeSpan(14, 0, 0),
                        HorarioFin = new TimeSpan(16, 0, 0),
                        Activo = true
                    }
                );
                await context.SaveChangesAsync();
            }

            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

            string roleName = "Coordinador";
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }

            string adminEmail = "coordinador@universidad.edu";
            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                var adminUser = new IdentityUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                await userManager.CreateAsync(adminUser, "Password123!");
                await userManager.AddToRoleAsync(adminUser, roleName);
            }
        }
    }
}