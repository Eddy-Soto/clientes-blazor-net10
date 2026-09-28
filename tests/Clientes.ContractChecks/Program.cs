using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using Clientes.Contracts;
using Original = Clientes.Api.Contracts.ClienteRequest;

var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
var count = 0;
void Check(bool ok, string name) { if (!ok) throw new Exception(name); count++; }
bool Valid(object value) => Validator.TryValidateObject(value, new ValidationContext(value), [], true);
var frontend = new Cliente
{
    IdCliente = 27, CUI = "9000000000001", NIT = "1234567-8", Nombres = "Ana",
    Apellidos = "Prueba", Direccion = "Dirección de prueba", Telefono = "+502 5555 1234",
    FechaNacimiento = new DateOnly(1990, 1, 6)
};
var json = JsonSerializer.Serialize(frontend.ToRequest(), options);
Check(!json.Contains("id_cliente"), "No enviar ID en POST/PUT");
var request = JsonSerializer.Deserialize<Original>(json, options)!;
Check(Valid(request), "Payload aceptado por validaciones de la API original");
Check(request.CUI == frontend.CUI && request.Nombres == frontend.Nombres, "Mapeo CUI/Nombres");
Check(request.Fecha_Nacimiento == frontend.FechaNacimiento, "Mapeo fecha_Nacimiento DateOnly");
var originalResponse = new Clientes.Api.Models.Cliente
{
    Id_cliente = 27, CUI = request.CUI, NIT = request.NIT, Nombres = request.Nombres,
    Apellidos = request.Apellidos, Direccion = request.Direccion, Telefono = request.Telefono,
    Fecha_Nacimiento = request.Fecha_Nacimiento
};
var read = JsonSerializer.Deserialize<Cliente>(JsonSerializer.Serialize(originalResponse, options), options)!;
Check(read.IdCliente == 27 && read.FechaNacimiento == frontend.FechaNacimiento, "Respuesta original leída por Blazor");
Check(Valid(frontend), "Formulario válido");
frontend.CUI = "123";
Check(!Valid(frontend), "CUI corto inválido");
frontend.CUI = "ABCDEFGHIJKLM";
Check(!Valid(frontend), "CUI alfabético inválido");
frontend.CUI = "9000000000001";
frontend.FechaNacimiento = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(1);
Check(!Valid(frontend), "Fecha futura inválida");
frontend.FechaNacimiento = null;
Check(!Valid(frontend), "Fecha vacía inválida");
frontend.FechaNacimiento = new DateOnly(1990, 1, 6);
frontend.Direccion = " ";
Check(!Valid(frontend), "Dirección requerida");
frontend.Direccion = "Prueba";
frontend.Nombres = new string('a', 101);
Check(!Valid(frontend), "Límite de nombres");
Console.WriteLine($"OK: {count} verificaciones de contrato y validación contra los DTO originales.");

