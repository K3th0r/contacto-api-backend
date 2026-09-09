namespace ContactosApi.DTOs
{
    public record ActualizarContactoRequest(
        string? Nombre,
        string? Telefono
    );
}
