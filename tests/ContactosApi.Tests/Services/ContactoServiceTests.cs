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

        [Fact]
        public void Actualizar_DeberiaActualizarContacto_ConservarIdYRecortarCampos()
        {
            var repository = new InMemoryContactoRepository();
            var service = new ContactoService(repository, NullLogger<ContactoService>.Instance);
            var creado = service.Crear(new CrearContactoRequest("Ana", "111111111"));

            var result = service.Actualizar(
                creado.Value!.Id,
                new ActualizarContactoRequest("  Ana Pérez  ", "  +56 9 9876 5432  "));

            result.IsSuccess.Should().BeTrue();
            result.Value!.Id.Should().Be(creado.Value.Id);
            result.Value.Nombre.Should().Be("Ana Pérez");
            result.Value.Telefono.Should().Be("+56 9 9876 5432");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Actualizar_DeberiaRetornarValidation_CuandoNombreEsInvalido(string? nombre)
        {
            var repository = new InMemoryContactoRepository();
            var service = new ContactoService(repository, NullLogger<ContactoService>.Instance);
            var creado = service.Crear(new CrearContactoRequest("Ana", "111111111"));

            var result = service.Actualizar(
                creado.Value!.Id,
                new ActualizarContactoRequest(nombre, "222222222"));

            result.IsSuccess.Should().BeFalse();
            result.ErrorType.Should().Be(ErrorType.Validation);
            result.Error.Should().Be("El nombre es obligatorio.");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Actualizar_DeberiaRetornarValidation_CuandoTelefonoEsInvalido(string? telefono)
        {
            var repository = new InMemoryContactoRepository();
            var service = new ContactoService(repository, NullLogger<ContactoService>.Instance);
            var creado = service.Crear(new CrearContactoRequest("Ana", "111111111"));

            var result = service.Actualizar(
                creado.Value!.Id,
                new ActualizarContactoRequest("Ana Pérez", telefono));

            result.IsSuccess.Should().BeFalse();
            result.ErrorType.Should().Be(ErrorType.Validation);
            result.Error.Should().Be("El teléfono es obligatorio.");
        }

        [Fact]
        public void Actualizar_DeberiaRetornarNotFound_CuandoContactoNoExiste()
        {
            var repository = new InMemoryContactoRepository();
            var service = new ContactoService(repository, NullLogger<ContactoService>.Instance);

            var result = service.Actualizar(999, new ActualizarContactoRequest("Ana", "111111111"));

            result.IsSuccess.Should().BeFalse();
            result.ErrorType.Should().Be(ErrorType.NotFound);
            result.Error.Should().Be("Contacto no encontrado.");
        }

        [Fact]
        public void Actualizar_DeberiaRetornarConflict_CuandoTelefonoPerteneceAOtroContacto()
        {
            var repository = new InMemoryContactoRepository();
            var service = new ContactoService(repository, NullLogger<ContactoService>.Instance);
            var primero = service.Crear(new CrearContactoRequest("Ana", "111111111"));
            var segundo = service.Crear(new CrearContactoRequest("Beto", "222222222"));

            var result = service.Actualizar(
                primero.Value!.Id,
                new ActualizarContactoRequest("Ana Pérez", segundo.Value!.Telefono));

            result.IsSuccess.Should().BeFalse();
            result.ErrorType.Should().Be(ErrorType.Conflict);
            result.Error.Should().Be("Ya existe un contacto con el mismo teléfono.");
        }

        [Fact]
        public void Actualizar_DeberiaPermitirMantenerElTelefonoDelMismoContacto()
        {
            var repository = new InMemoryContactoRepository();
            var service = new ContactoService(repository, NullLogger<ContactoService>.Instance);
            var creado = service.Crear(new CrearContactoRequest("Ana", "111111111"));

            var result = service.Actualizar(
                creado.Value!.Id,
                new ActualizarContactoRequest("Ana Pérez", "111111111"));

            result.IsSuccess.Should().BeTrue();
            result.Value!.Telefono.Should().Be("111111111");
        }

        [Fact]
        public async Task Actualizar_DeberiaPermitirSoloUnContacto_CuandoDosActualizacionesParalelasUsanMismoTelefono()
        {
            var repository = new InMemoryContactoRepository();
            var service = new ContactoService(repository, NullLogger<ContactoService>.Instance);
            var primero = service.Crear(new CrearContactoRequest("Ana", "111111111"));
            var segundo = service.Crear(new CrearContactoRequest("Beto", "222222222"));
            using var start = new Barrier(2);

            var results = await Task.WhenAll(
                Task.Run(() =>
                {
                    start.SignalAndWait();
                    return service.Actualizar(primero.Value!.Id, new ActualizarContactoRequest("Ana", "999999999"));
                }),
                Task.Run(() =>
                {
                    start.SignalAndWait();
                    return service.Actualizar(segundo.Value!.Id, new ActualizarContactoRequest("Beto", "999999999"));
                }));

            results.Count(result => result.IsSuccess).Should().Be(1);
            results.Count(result => result.ErrorType == ErrorType.Conflict).Should().Be(1);
            repository.GetAll().Count(contacto => contacto.Telefono == "999999999").Should().Be(1);
        }
    }
}
