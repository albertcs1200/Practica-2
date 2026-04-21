using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PortalUniversidad.Models;

public class Curso : IValidatableObject
{
    public int Id { get; set; }

    [Required]
    [StringLength(20)]
    public string Codigo { get; set; } = null!;

    [Required]
    [StringLength(100)]
    public string Nombre { get; set; } = null!;

    [Range(1, int.MaxValue, ErrorMessage = "Los créditos no pueden ser negativos ni cero.")]
    public int Creditos { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "El cupo máximo debe ser mayor a 0.")]
    public int CupoMaximo { get; set; }

    [Required]
    public TimeSpan HorarioInicio { get; set; }

    [Required]
    public TimeSpan HorarioFin { get; set; }

    public bool Activo { get; set; }

    public ICollection<Matricula> Matriculas { get; set; } = new List<Matricula>();

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (HorarioFin <= HorarioInicio)
        {
            yield return new ValidationResult(
                "El horario de fin debe ser posterior al horario de inicio.",
                new[] { nameof(HorarioFin), nameof(HorarioInicio) });
        }
    }
}
