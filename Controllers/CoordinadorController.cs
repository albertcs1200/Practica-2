using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortalUniversidad.Data;
using PortalUniversidad.Models;

namespace PortalUniversidad.Controllers;

[Authorize(Roles = "Coordinador")]
public class CoordinadorController(ApplicationDbContext context) : Controller
{
    // GET: Coordinador
    public async Task<IActionResult> Index()
    {
        var cursos = await context.Cursos.ToListAsync();
        return View(cursos);
    }

    // GET: Coordinador/Crear
    public IActionResult Crear()
    {
        return View();
    }

    // POST: Coordinador/Crear
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Crear([Bind("Codigo,Nombre,Creditos,CupoMaximo,HorarioInicio,HorarioFin,Activo")] Curso curso)
    {
        if (ModelState.IsValid)
        {
            // Validar unicidad del código
            if (await context.Cursos.AnyAsync(c => c.Codigo == curso.Codigo))
            {
                ModelState.AddModelError("Codigo", "El código del curso ya existe.");
                return View(curso);
            }

            context.Add(curso);
            await context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(curso);
    }

    // GET: Coordinador/Editar/5
    public async Task<IActionResult> Editar(int? id)
    {
        if (id == null) return NotFound();

        var curso = await context.Cursos.FindAsync(id);
        if (curso == null) return NotFound();
        return View(curso);
    }

    // POST: Coordinador/Editar/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Editar(int id, [Bind("Id,Codigo,Nombre,Creditos,CupoMaximo,HorarioInicio,HorarioFin,Activo")] Curso curso)
    {
        if (id != curso.Id) return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                var existeCodigo = await context.Cursos.AnyAsync(c => c.Codigo == curso.Codigo && c.Id != curso.Id);
                if (existeCodigo)
                {
                    ModelState.AddModelError("Codigo", "El código del curso ya existe.");
                    return View(curso);
                }

                context.Update(curso);
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!context.Cursos.Any(e => e.Id == curso.Id)) return NotFound();
                else throw;
            }
            return RedirectToAction(nameof(Index));
        }
        return View(curso);
    }

    // POST: Coordinador/AlternarEstado/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AlternarEstado(int id)
    {
        var curso = await context.Cursos.FindAsync(id);
        if (curso == null) return NotFound();

        curso.Activo = !curso.Activo;
        await context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    // GET: Coordinador/Matriculas/5
    public async Task<IActionResult> Matriculas(int? id)
    {
        if (id == null) return NotFound();

        var curso = await context.Cursos.FindAsync(id);
        if (curso == null) return NotFound();

        var matriculas = await context.Matriculas
            .Include(m => m.Usuario)
            .Where(m => m.CursoId == id)
            .ToListAsync();

        ViewBag.CursoNombre = curso.Nombre;
        ViewBag.CursoId = curso.Id;
        return View(matriculas);
    }

    // POST: Coordinador/CambiarEstadoMatricula
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CambiarEstadoMatricula(int matriculaId, EstadoMatricula nuevoEstado, int cursoId)
    {
        var matricula = await context.Matriculas.FindAsync(matriculaId);
        if (matricula == null) return NotFound();

        matricula.Estado = nuevoEstado;
        await context.SaveChangesAsync();

        return RedirectToAction(nameof(Matriculas), new { id = cursoId });
    }
}
