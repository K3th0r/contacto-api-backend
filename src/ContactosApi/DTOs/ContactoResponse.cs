namespace ContactosApi.DTOs
{

    //DTO DE SALIDA
    //datos devuelta al cliente
    public record ContactoResponse(
        int Id,
        string Nombre,
        string Telefono
        );

}
