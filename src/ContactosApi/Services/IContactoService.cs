using ContactosApi.Common;
using ContactosApi.DTOs;

namespace ContactosApi.Services
{

    //interfaz, solo definicion de operaciones
    public interface IContactoService
    {
        IReadOnlyCollection<ContactoResponse> ObtenerTodos();
        Result<ContactoResponse> ObtenerPorId(int id);
        Result<ContactoResponse> Crear(CrearContactoRequest request);
        Result<ContactoResponse> Actualizar(int id, ActualizarContactoRequest request);
    }

}
