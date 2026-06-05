using ContactosApi.Common;
using ContactosApi.DTOs;
using ContactosApi.Repositories;
using ContactosApi.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;

namespace ContactosApi.Tests.Services
{
    public class ContactoServiceTests
    {
        [Fact]
        public void Crear_DeberiaRetornarConflict_CuandoTelefonoYaExiste()
        {
            //VALIDAR REGLA DE NEGOCIO (TELEFONOS DUPLICADOS) SIN LEVANTAR API COMPLETA


            // Arrange
            var repository = new InMemoryContactoRepository();
            var logger = NullLogger<ContactoService>.Instance;
            var service = new ContactoService(repository, logger);

            var request = new CrearContactoRequest("Juan Perez", "123456789");
            var duplicateRequest = new CrearContactoRequest("Pedro Soto", "123456789");

            service.Crear(request);

            // Act
            var result = service.Crear(duplicateRequest);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.ErrorType.Should().Be(ErrorType.Conflict);
            result.Error.Should().Be("Ya existe un contacto con el mismo teléfono.");
        }
    }
}
