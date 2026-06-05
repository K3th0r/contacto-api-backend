using ContactosApi.Common;
using ContactosApi.Domain;
using ContactosApi.DTOs;
using ContactosApi.Repositories;

namespace ContactosApi.Services
{

    //servicio con reglas de negocio, mantiene controller enfocado en http
    public sealed class ContactoService : IContactoService
    {

        private readonly IContactoRepository _repository;
        private readonly ILogger<ContactoService> _logger;

        public ContactoService(
            IContactoRepository repository,
            ILogger<ContactoService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public IReadOnlyCollection<ContactoResponse> ObtenerTodos()
        {
            return _repository.GetAll() //invoca al repositorio y si hay respuesta devuelve la lista, en caso contrario es lista vacia
                .Select(ToResponse)
                .ToList();
        }

        public Result<ContactoResponse> ObtenerPorId(int id)
        {
            var contacto = _repository.GetById(id);

            if (contacto is null)
                return Result<ContactoResponse>.NotFound("Contacto no encontrado."); //NotFound, se traduce en 404 mas adelante

            return Result<ContactoResponse>.Success(ToResponse(contacto));
        }

        public Result<ContactoResponse> Crear(CrearContactoRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Nombre))
                return Result<ContactoResponse>.Validation("El nombre es obligatorio.");

            if (string.IsNullOrWhiteSpace(request.Telefono))
                return Result<ContactoResponse>.Validation("El teléfono es obligatorio.");

            var nombre = request.Nombre.Trim();
            var telefono = request.Telefono.Trim();

            var existingContact = _repository.GetByTelefono(telefono);

            if (existingContact is not null)
                return Result<ContactoResponse>.Conflict("Ya existe un contacto con el mismo teléfono.");

            try
            {
                var contacto = _repository.Add(nombre, telefono);


                //LOGGING CON PLACEHOLDER
                _logger.LogInformation(
                    "Contacto creado correctamente. ContactoId: {ContactoId}",
                    contacto.Id);

                return Result<ContactoResponse>.Success(ToResponse(contacto));
            }
            catch (InvalidOperationException)
            {
                //existe una segunda validacion dentro del repositorio en caso de que dos solicitudes paralelas pasen la validacion de contacto creado con el mismo telefono
                //caeria aca en caso de conflicto
                return Result<ContactoResponse>.Conflict("Ya existe un contacto con el mismo teléfono.");
            }
        }

        private static ContactoResponse ToResponse(Contacto contacto)
        {
            //mapeo desde dominio a salida como DTO
            return new ContactoResponse(
                contacto.Id,
                contacto.Nombre,
                contacto.Telefono
            );
        }
    }
}
