using System;
using Microsoft.AspNetCore.Identity;

namespace PortalUniversidad.Models;

public class Matricula
{
    public int Id { get; set; }

    public int CursoId { get; set; }
    public Curso Curso { get; set; } = null!;

    public string UsuarioId { get; set; } = null!;
    public IdentityUser Usuario { get; set; } = null!;

    public DateTime FechaRegistro { get; set; }

    public EstadoMatricula Estado { get; set; }
}
