using System.Collections.Concurrent;
using ContactosApi.Domain;

namespace ContactosApi.Repositories
{

    //almacenamiento en memoria
    //se usa una coleccion concurrente
    //Se desacoplan detalles de almacenamiento de la capa de servicio y controlador.
    public sealed class InMemoryContactoRepository : IContactoRepository
    {
        private readonly ConcurrentDictionary<int, Contacto> _contactos = new();
        private readonly object _writeLock = new(); //CONCURRENCIA

        private int _nextId = 0;

        public IReadOnlyCollection<Contacto> GetAll()
        {
            return _contactos.Values
                .OrderBy(contacto => contacto.Id) //RETORNA TODO
                .ToList();
        }

        public Contacto? GetById(int id)
        {
            _contactos.TryGetValue(id, out var contacto); //RETORNA X ID
            return contacto;
        }

        public Contacto? GetByTelefono(string telefono)
        {
            return _contactos.Values.FirstOrDefault(contacto =>
                string.Equals(contacto.Telefono, telefono, StringComparison.OrdinalIgnoreCase)); //PERMITE BUSCAR TELEFONOS EXISTENTES
        }

        public Contacto Add(string nombre, string telefono)
        {
            lock (_writeLock) //THREAD SAFE
            {
                var existingContact = GetByTelefono(telefono);

                if (existingContact is not null)
                    throw new InvalidOperationException("Ya existe un contacto con el mismo teléfono.");

                var id = Interlocked.Increment(ref _nextId); //ID AUTOMATICO INTERNO GENERADO DENTRO DEL SEGURO ANTE CONCURRENCIA
                var contacto = new Contacto(id, nombre, telefono);

                _contactos.TryAdd(contacto.Id, contacto);

                return contacto;
            }
        }

        public Contacto? Update(int id, string nombre, string telefono)
        {
            lock (_writeLock)
            {
                if (!_contactos.TryGetValue(id, out var contacto))
                    return null;

                var existingContact = GetByTelefono(telefono);

                if (existingContact is not null && existingContact.Id != id)
                    throw new InvalidOperationException("Ya existe un contacto con el mismo teléfono.");

                var updatedContact = contacto with
                {
                    Nombre = nombre,
                    Telefono = telefono
                };

                _contactos[id] = updatedContact;

                return updatedContact;
            }
        }
    }
}
