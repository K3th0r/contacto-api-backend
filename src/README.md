# API de Contactos

Una API sencilla para guardar y consultar contactos. Permite crear contactos,
ver la lista disponible y buscar uno por su identificador.

Al iniciar el proyecto, la API queda disponible en:

`http://localhost:5210`

Tambien puedes probarla desde Swagger, una pantalla interactiva incluida en el
proyecto:

`http://localhost:5210/swagger`

## Iniciar la API

Desde la raiz del proyecto ejecuta:

```bash
dotnet run --project ContactosApi
```

## Endpoints disponibles

### Ver todos los contactos

`GET /api/contactos`

Devuelve la lista de contactos creados. Si todavia no hay contactos, devuelve
una lista vacia.

Ejemplo:

```bash
curl http://localhost:5210/api/contactos
```

Respuesta de ejemplo:

```json
[
  {
    "id": 1,
    "nombre": "Ana Perez",
    "telefono": "+56 9 1234 5678"
  }
]
```

Uso util: mostrar una agenda de contactos en una aplicacion o revisar los
contactos que ya se registraron.

### Ver un contacto por ID

`GET /api/contactos/{id}`

Busca un contacto usando su ID. Reemplaza `{id}` por el numero del contacto.

Ejemplo:

```bash
curl http://localhost:5210/api/contactos/1
```

Uso util: ver los datos de una persona seleccionada desde una lista de
contactos.

Si el ID no existe, la API informa que el contacto no fue encontrado.

### Crear un contacto

`POST /api/contactos`

Crea un contacto nuevo. Debes enviar su nombre y telefono.

Ejemplo:

```bash
curl -X POST http://localhost:5210/api/contactos \
  -H "Content-Type: application/json" \
  -d '{"nombre":"Ana Perez","telefono":"+56 9 1234 5678"}'
```

Respuesta de ejemplo:

```json
{
  "id": 1,
  "nombre": "Ana Perez",
  "telefono": "+56 9 1234 5678"
}
```

Uso util: agregar una persona a la agenda desde un formulario de registro.

## Datos necesarios al crear un contacto

| Campo | Para que sirve | Es obligatorio |
| --- | --- | --- |
| `nombre` | Nombre de la persona | Si |
| `telefono` | Telefono de contacto | Si |

No se puede registrar dos veces el mismo telefono. Si falta un dato o el
telefono ya existe, la API devuelve un mensaje explicando el problema.

## Filtros y busqueda

Por ahora, la lista de contactos no tiene filtros por nombre, telefono u otros
datos. El unico dato usado para buscar es el **ID** en la direccion, por ejemplo
`/api/contactos/1`.

## Importante

Los contactos se guardan solo mientras la API esta en ejecucion. Al detener o
reiniciar el proyecto, la lista vuelve a estar vacia.
