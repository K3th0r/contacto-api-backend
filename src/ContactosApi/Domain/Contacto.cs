namespace ContactosApi.Domain
{

    // entidad contacto, se usa record como bonus, representa modelo inmutable simple
    public record Contacto(
        int Id,
        string Nombre,
        string Telefono
    );

}
