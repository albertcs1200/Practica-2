using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PortalUniversidad.Models;

namespace PortalUniversidad.Data;

public static class SeedData
{
    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
        var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

        await context.Database.MigrateAsync();

        // Crear rol Coordinador si no existe
        if (!await roleManager.RoleExistsAsync("Coordinador"))
        {
            await roleManager.CreateAsync(new IdentityRole("Coordinador"));
        }

        // Crear usuario coordinador
        if (await userManager.FindByEmailAsync("admin@universidad.edu") == null)
        {
            var user = new IdentityUser
            {
                UserName = "admin@universidad.edu",
                Email = "admin@universidad.edu",
                EmailConfirmed = true
            };
            var result = await userManager.CreateAsync(user, "Password123!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, "Coordinador");
            }
        }

        // Cursos semilla
        if (!context.Cursos.Any())
        {
            context.Cursos.AddRange(
                new Curso
                {
                    Codigo = "MAT101",
                    Nombre = "Matemáticas Básicas",
                    Creditos = 4,
                    CupoMaximo = 30,
                    HorarioInicio = new TimeSpan(8, 0, 0),
                    HorarioFin = new TimeSpan(10, 0, 0),
                    Activo = true
                },
                new Curso
                {
                    Codigo = "PROG101",
                    Nombre = "Programación I",
                    Creditos = 5,
                    CupoMaximo = 25,
                    HorarioInicio = new TimeSpan(10, 0, 0),
                    HorarioFin = new TimeSpan(12, 0, 0),
                    Activo = true
                },
                new Curso
                {
                    Codigo = "FIS101",
                    Nombre = "Física General",
                    Creditos = 4,
                    CupoMaximo = 30,
                    HorarioInicio = new TimeSpan(14, 0, 0),
                    HorarioFin = new TimeSpan(16, 0, 0),
                    Activo = true
                }
            );
            await context.SaveChangesAsync();
        }
    }
}
