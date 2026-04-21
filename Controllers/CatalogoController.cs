using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortalUniversidad.Data;
using PortalUniversidad.Models;

namespace PortalUniversidad.Controllers;

public class CatalogoController(ApplicationDbContext context, UserManager<IdentityUser> userManager) : Controller
{
    public async Task<IActionResult> Index(string? buscarNombre, int? minCreditos, int? maxCreditos, TimeSpan? horarioFiltro)
    {
        var query = context.Cursos.Where(c => c.Activo).AsQueryable();

        if (!string.IsNullOrEmpty(buscarNombre))
        {
            query = query.Where(c => c.Nombre.Contains(buscarNombre));
        }

        if (minCreditos.HasValue)
        {
            query = query.Where(c => c.Creditos >= minCreditos.Value);
        }

        if (maxCreditos.HasValue)
        {
            query = query.Where(c => c.Creditos <= maxCreditos.Value);
        }

        if (horarioFiltro.HasValue)
        {
            query = query.Where(c => c.HorarioInicio <= horarioFiltro.Value && c.HorarioFin >= horarioFiltro.Value);
        }

        var cursos = await query.ToListAsync();

        ViewData["BuscarNombre"] = buscarNombre;
        ViewData["MinCreditos"] = minCreditos;
        ViewData["MaxCreditos"] = maxCreditos;
        ViewData["HorarioFiltro"] = horarioFiltro?.ToString(@"hh\:mm");

        return View(cursos);
    }

    public async Task<IActionResult> Detalle(int id)
    {
        var curso = await context.Cursos.FirstOrDefaultAsync(c => c.Id == id && c.Activo);
        
        if (curso == null)
        {
            return NotFound();
        }

        return View(curso);
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Inscribirse(int id)
    {
        var curso = await context.Cursos.FirstOrDefaultAsync(c => c.Id == id && c.Activo);
        if (curso == null)
        {
            return NotFound();
        }

        var user = await userManager.GetUserAsync(User);
        if (user == null)
        {
            return Challenge();
        }

        // Validar si ya está matriculado
        var yaMatriculado = await context.Matriculas
            .AnyAsync(m => m.CursoId == id && m.UsuarioId == user.Id && m.Estado != EstadoMatricula.Cancelada);
            
        if (yaMatriculado)
        {
            TempData["Error"] = "Ya estás matriculado o tienes una solicitud pendiente para este curso.";
            return RedirectToAction(nameof(Detalle), new { id });
        }

        // Validar cupo
        var matriculadosCount = await context.Matriculas
            .CountAsync(m => m.CursoId == id && m.Estado != EstadoMatricula.Cancelada);
            
        if (matriculadosCount >= curso.CupoMaximo)
        {
            TempData["Error"] = "El curso ha alcanzado su cupo máximo.";
            return RedirectToAction(nameof(Detalle), new { id });
        }

        // Validar solapamiento de horarios
        var solapamiento = await context.Matriculas
            .Include(m => m.Curso)
            .Where(m => m.UsuarioId == user.Id && m.Estado != EstadoMatricula.Cancelada)
            .AnyAsync(m => m.Curso.HorarioInicio < curso.HorarioFin && m.Curso.HorarioFin > curso.HorarioInicio);

        if (solapamiento)
        {
            TempData["Error"] = "El horario del curso se solapa con otro curso en el que ya estás matriculado.";
            return RedirectToAction(nameof(Detalle), new { id });
        }

        var matricula = new Matricula
        {
            CursoId = curso.Id,
            UsuarioId = user.Id,
            FechaRegistro = DateTime.UtcNow,
            Estado = EstadoMatricula.Pendiente
        };

        context.Matriculas.Add(matricula);
        await context.SaveChangesAsync();

        TempData["Exito"] = "Inscripción realizada con éxito. Su matrícula está en estado Pendiente.";
        return RedirectToAction(nameof(Detalle), new { id });
    }
}
