namespace Clientes.Api.Models;

public class Cliente
{
    public int Id_cliente { get; set; }
    public string CUI { get; set; } = "";
    public string NIT { get; set; } = "";
    public string Nombres { get; set; } = "";
    public string Apellidos { get; set; } = "";
    public string Direccion { get; set; } = "";
    public string Telefono { get; set; } = "";
    public DateOnly Fecha_Nacimiento { get; set; }
}
