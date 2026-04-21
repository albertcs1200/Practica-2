using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortalUniversidad.Data;
using PortalUniversidad.Models;

namespace PortalUniversidad.Controllers;

public class CatalogoController(ApplicationDbContext context) : Controller
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
            // Filtra cursos donde el horario filtro se encuentre dentro del rango del curso
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
    [ValidateAntiForgeryToken]
    public IActionResult Inscribirse(int id)
    {
        // El botón Inscribirse se incluye en la vista, 
        // temporalmente dejamos un mensaje ya que la lógica 
        // fuerte de inscripción suele ir en un paso posterior o requiere estar autenticado.
        TempData["Mensaje"] = "¡Botón Inscribirse presionado! La lógica de matrícula está lista para implementarse.";
        return RedirectToAction(nameof(Detalle), new { id });
    }
}
