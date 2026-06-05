using ContactosApi.Common;
using ContactosApi.DTOs;
using ContactosApi.Repositories;
using ContactosApi.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Logging.Abstractions;
using System.Net;
using System.Net.Http.Json;

namespace ContactosApi.Tests
{
    public class ContactosApiIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public ContactosApiIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task PostContactos_DeberiaCrearContacto_YRetornarCreated()
        {
            //TEST DE INTEGRACION
            //CON API EN MEMORIA PRUEBA ENDPOINT POST DE CONTACTOS


            // Arrange
            var request = new CrearContactoRequest("Juan Perez", "123456789");

            // Act
            var response = await _client.PostAsJsonAsync("/api/contactos", request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);

            var contacto = await response.Content.ReadFromJsonAsync<ContactoResponse>();

            contacto.Should().NotBeNull();
            contacto!.Id.Should().BeGreaterThan(0);
            contacto.Nombre.Should().Be("Juan Perez");
            contacto.Telefono.Should().Be("123456789");
        }


        [Fact]
        public async Task Crear_DeberiaPermitirSoloUnContacto_CuandoSeAgregaMismoTelefonoEnParalelo()
        {
            // Simula varias solicitudes intentando crear contactos con el mismo telefono
            // solo una operación debe ser exitosa y las demás retornen conflicto.

            // Arrange
            var repository = new InMemoryContactoRepository();
            var logger = NullLogger<ContactoService>.Instance;
            var service = new ContactoService(repository, logger);

            var tasks = Enumerable.Range(1, 20)
                .Select(index => Task.Run(() =>
                    service.Crear(new CrearContactoRequest($"Usuario {index}", "999999999"))))
                .ToList();

            // Act
            var results = await Task.WhenAll(tasks);

            // Assert
            results.Count(result => result.IsSuccess).Should().Be(1);
            results.Count(result => result.ErrorType == ErrorType.Conflict).Should().Be(19);
        }
    }
}
