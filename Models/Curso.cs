using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PortalUniversidad.Models;

public class Curso
{
    public int Id { get; set; }

    [Required]
    [StringLength(20)]
    public string Codigo { get; set; } = null!;

    [Required]
    [StringLength(100)]
    public string Nombre { get; set; } = null!;

    public int Creditos { get; set; }

    public int CupoMaximo { get; set; }

    public TimeSpan HorarioInicio { get; set; }

    public TimeSpan HorarioFin { get; set; }

    public bool Activo { get; set; }

    public ICollection<Matricula> Matriculas { get; set; } = new List<Matricula>();
}
