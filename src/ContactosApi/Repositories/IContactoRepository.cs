using ContactosApi.Domain;

namespace ContactosApi.Repositories
{

    //uso de interfaces y testeable
    // permite cambiar implementacion de registro en memoria sin necsidad de modificar Service.
    public interface IContactoRepository
    {
        IReadOnlyCollection<Contacto> GetAll();
        Contacto? GetById(int id);
        Contacto? GetByTelefono(string telefono);
        Contacto Add(string nombre, string telefono);
    }
}
