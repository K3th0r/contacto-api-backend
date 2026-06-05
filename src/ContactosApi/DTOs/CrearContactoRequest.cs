namespace ContactosApi.DTOs
{

    //DTO para POST api/contacto
    // la entidad interna no se expone, se separa del dominio y se desacopla de la capa HTTP

    public record CrearContactoRequest(
        string? Nombre,
        string? Telefono
    );
}
