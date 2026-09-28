using System.ComponentModel.DataAnnotations;

namespace Clientes.Api.Contracts;

public class ClienteRequest : IValidatableObject
{
    [Required, RegularExpression(@"^[0-9]{13}$", ErrorMessage = "El CUI debe contener 13 dígitos.")]
    public string CUI { get; set; } = "";
    [Required, StringLength(20)]
    public string NIT { get; set; } = "";
    [Required, StringLength(100)]
    public string Nombres { get; set; } = "";
    [Required, StringLength(100)]
    public string Apellidos { get; set; } = "";
    [Required, StringLength(250)]
    public string Direccion { get; set; } = "";
    [Required, StringLength(25), Phone]
    public string Telefono { get; set; } = "";
    public DateOnly Fecha_Nacimiento { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Fecha_Nacimiento == default || Fecha_Nacimiento > DateOnly.FromDateTime(DateTime.UtcNow))
            yield return new ValidationResult("La fecha de nacimiento es obligatoria y no puede ser futura.", [nameof(Fecha_Nacimiento)]);
    }
}
