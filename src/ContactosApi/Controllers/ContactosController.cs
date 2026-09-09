using ContactosApi.Common;
using ContactosApi.DTOs;
using ContactosApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace ContactosApi.Controllers
{

    //controller, expone endpoint REST, centrado en respuestas HTTP

    [ApiController]
    [Route("api/contactos")]
    public sealed class ContactosController : ControllerBase
    {
        private readonly IContactoService _contactoService;

        public ContactosController(IContactoService contactoService)
        {
            _contactoService = contactoService;
        }



        //  RESPONDE 200 OK Y RETORNA LISTA COMPLETA AUN QUE VENGA VACIA
        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyCollection<ContactoResponse>), StatusCodes.Status200OK)]
        public ActionResult<IReadOnlyCollection<ContactoResponse>> ObtenerTodos()
        {
            var contactos = _contactoService.ObtenerTodos();

            return Ok(contactos);
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ContactoResponse), StatusCodes.Status200OK)] //200 si existe
        [ProducesResponseType(StatusCodes.Status404NotFound)] //404 si no existe
        public ActionResult<ContactoResponse> ObtenerPorId(int id)
        {
            var result = _contactoService.ObtenerPorId(id);

            if (!result.IsSuccess)
                return NotFound(new { message = result.Error });

            return Ok(result.Value);
        }

        [HttpPost]
        [ProducesResponseType(typeof(ContactoResponse), StatusCodes.Status201Created)] //201 si crea
        [ProducesResponseType(StatusCodes.Status400BadRequest)] // 400 bad request, faltan campos
        [ProducesResponseType(StatusCodes.Status409Conflict)] //409 conflict cuando telefono ya existe
        public ActionResult<ContactoResponse> Crear(CrearContactoRequest request)
        {
            var result = _contactoService.Crear(request);

            if (!result.IsSuccess)
            {
                return result.ErrorType switch
                {
                    ErrorType.Validation => BadRequest(new { message = result.Error }),
                    ErrorType.Conflict => Conflict(new { message = result.Error }),
                    _ => BadRequest(new { message = result.Error })
                };
            }

            return CreatedAtAction(
                nameof(ObtenerPorId),
                new { id = result.Value!.Id },
                result.Value
            );
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(ContactoResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public ActionResult<ContactoResponse> Actualizar(int id, ActualizarContactoRequest request)
        {
            var result = _contactoService.Actualizar(id, request);

            if (!result.IsSuccess)
            {
                return result.ErrorType switch
                {
                    ErrorType.Validation => BadRequest(new { message = result.Error }),
                    ErrorType.NotFound => NotFound(new { message = result.Error }),
                    ErrorType.Conflict => Conflict(new { message = result.Error }),
                    _ => BadRequest(new { message = result.Error })
                };
            }

            return Ok(result.Value);
        }
    }
}
