using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Clientes.Contracts;

// DTO independiente: el frontend no referencia EF Core ni el proyecto de la API.
public class ClienteRequest : IValidatableObject
{
    [JsonPropertyName("cui")]
    [Required(ErrorMessage = "El CUI es obligatorio.")]
    [RegularExpression(@"^[0-9]{13}$", ErrorMessage = "El CUI debe contener 13 dígitos.")]
    public string CUI { get; set; } = "";

    [JsonPropertyName("nit")]
    [Required(ErrorMessage = "El NIT es obligatorio."), StringLength(20, ErrorMessage = "El NIT admite hasta 20 caracteres.")]
    public string NIT { get; set; } = "";

    [JsonPropertyName("nombres")]
    [Required(ErrorMessage = "Los nombres son obligatorios."), StringLength(100)]
    public string Nombres { get; set; } = "";

    [JsonPropertyName("apellidos")]
    [Required(ErrorMessage = "Los apellidos son obligatorios."), StringLength(100)]
    public string Apellidos { get; set; } = "";

    [JsonPropertyName("direccion")]
    [Required(ErrorMessage = "La dirección es obligatoria."), StringLength(250)]
    public string Direccion { get; set; } = "";

    [JsonPropertyName("telefono")]
    [Required(ErrorMessage = "El teléfono es obligatorio."), StringLength(25), Phone(ErrorMessage = "Escribe un teléfono válido.")]
    public string Telefono { get; set; } = "";

    [JsonPropertyName("fecha_Nacimiento")]
    [Required(ErrorMessage = "La fecha de nacimiento es obligatoria.")]
    public DateOnly? FechaNacimiento { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (FechaNacimiento is { } fecha && (fecha == default || fecha > DateOnly.FromDateTime(DateTime.UtcNow)))
            yield return new ValidationResult("La fecha de nacimiento no puede ser futura ni vacía.", [nameof(FechaNacimiento)]);
    }
}

public class Cliente : ClienteRequest
{
    [JsonPropertyName("id_cliente")]
    public int IdCliente { get; set; }

    // POST y PUT envían únicamente los campos de ClienteRequest del contrato original.
    public ClienteRequest ToRequest() => new()
    {
        CUI = CUI.Trim(), NIT = NIT.Trim(), Nombres = Nombres.Trim(),
        Apellidos = Apellidos.Trim(), Direccion = Direccion.Trim(),
        Telefono = Telefono.Trim(), FechaNacimiento = FechaNacimiento
    };
}

