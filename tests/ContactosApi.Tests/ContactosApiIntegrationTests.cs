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

        [Fact]
        public async Task PutContactos_DeberiaActualizarContacto_YObtenerPorIdDeberiaRetornarValoresActualizados()
        {
            var creado = await CrearContactoAsync("Ana", NuevoTelefono());
            var request = new ActualizarContactoRequest("  Ana Pérez  ", "  +56 9 9876 5432  ");

            var updateResponse = await _client.PutAsJsonAsync($"/api/contactos/{creado.Id}", request);

            updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var actualizado = await updateResponse.Content.ReadFromJsonAsync<ContactoResponse>();
            actualizado.Should().NotBeNull();
            actualizado!.Id.Should().Be(creado.Id);
            actualizado.Nombre.Should().Be("Ana Pérez");
            actualizado.Telefono.Should().Be("+56 9 9876 5432");

            var getResponse = await _client.GetAsync($"/api/contactos/{creado.Id}");

            getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var obtenido = await getResponse.Content.ReadFromJsonAsync<ContactoResponse>();
            obtenido.Should().Be(actualizado);
        }

        [Fact]
        public async Task PutContactos_DeberiaRetornarNotFound_CuandoIdNoExiste()
        {
            var response = await _client.PutAsJsonAsync(
                "/api/contactos/999999",
                new ActualizarContactoRequest("Ana", NuevoTelefono()));

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task PutContactos_DeberiaRetornarBadRequest_CuandoCamposSonVaciosOMissing()
        {
            var creado = await CrearContactoAsync("Ana", NuevoTelefono());

            var blankResponse = await _client.PutAsJsonAsync(
                $"/api/contactos/{creado.Id}",
                new ActualizarContactoRequest("   ", "   "));
            var missingResponse = await _client.PutAsync(
                $"/api/contactos/{creado.Id}",
                new StringContent("{}", System.Text.Encoding.UTF8, "application/json"));

            blankResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            missingResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            var error = await blankResponse.Content.ReadFromJsonAsync<Dictionary<string, string?>>();
            error.Should().ContainKey("message");
            error!["message"].Should().Be("El nombre es obligatorio.");
        }

        [Fact]
        public async Task PutContactos_DeberiaRetornarConflict_CuandoTelefonoPerteneceAOtroContacto()
        {
            var primero = await CrearContactoAsync("Ana", NuevoTelefono());
            var segundo = await CrearContactoAsync("Beto", NuevoTelefono());

            var response = await _client.PutAsJsonAsync(
                $"/api/contactos/{primero.Id}",
                new ActualizarContactoRequest("Ana", segundo.Telefono));

            response.StatusCode.Should().Be(HttpStatusCode.Conflict);
        }

        [Fact]
        public async Task PutContactos_NoDeberiaCoincidirConRutaCuandoIdNoEsEntero()
        {
            var response = await _client.PutAsJsonAsync(
                "/api/contactos/no-es-entero",
                new ActualizarContactoRequest("Ana", NuevoTelefono()));

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        private async Task<ContactoResponse> CrearContactoAsync(string nombre, string telefono)
        {
            var response = await _client.PostAsJsonAsync(
                "/api/contactos",
                new CrearContactoRequest(nombre, telefono));

            response.StatusCode.Should().Be(HttpStatusCode.Created);
            return (await response.Content.ReadFromJsonAsync<ContactoResponse>())!;
        }

        private static string NuevoTelefono()
        {
            return $"9{Guid.NewGuid():N}";
        }
    }
}
