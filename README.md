# API de Contactos

API REST desarrollada con .NET 8 para crear, consultar y actualizar contactos.
Está diseñada para ejecutarse localmente, sin base de datos ni servicios externos.

## Requisitos

- .NET 8 SDK o una versión posterior compatible con `net8.0`.

```bash
dotnet --version
```

## Ejecutar la API

Desde la raíz del repositorio:

```bash
dotnet run --project src/ContactosApi/ContactosApi.csproj
```

La configuración local expone la API en `http://localhost:5210`.

Swagger está disponible en:

```text
http://localhost:5210/swagger
```

## Ejecutar las pruebas

Desde la raíz del repositorio:

```bash
dotnet test
```

## Endpoints

| Método | Ruta | Resultado exitoso |
| --- | --- | --- |
| `GET` | `/api/contactos` | `200 OK` con todos los contactos, incluso si la lista está vacía. |
| `GET` | `/api/contactos/{id}` | `200 OK` con el contacto solicitado. |
| `POST` | `/api/contactos` | `201 Created` con el contacto creado. |
| `PUT` | `/api/contactos/{id}` | `200 OK` con el contacto actualizado. |

### Obtener todos los contactos

```http
GET /api/contactos
```

Ejemplo de respuesta:

```json
[
  {
    "id": 1,
    "nombre": "Ana Pérez",
    "telefono": "+56 9 1234 5678"
  }
]
```

### Obtener un contacto por ID

```http
GET /api/contactos/{id}
```

```bash
curl http://localhost:5210/api/contactos/1
```

Si el ID no existe, la API responde `404 Not Found`.

### Crear un contacto

```http
POST /api/contactos
Content-Type: application/json
```

Solicitud de ejemplo:

```json
{
  "nombre": "Ana Pérez",
  "telefono": "+56 9 1234 5678"
}
```

```bash
curl -X POST http://localhost:5210/api/contactos \
  -H "Content-Type: application/json" \
  -d '{"nombre":"Ana Pérez","telefono":"+56 9 1234 5678"}'
```

Respuesta de ejemplo (`201 Created`):

```json
{
  "id": 1,
  "nombre": "Ana Pérez",
  "telefono": "+56 9 1234 5678"
}
```

### Actualizar un contacto

`PUT` reemplaza los campos editables de un contacto existente y conserva su ID.
`nombre` y `telefono` son obligatorios; se eliminan sus espacios iniciales y
finales antes de guardarlos.

```http
PUT /api/contactos/{id}
Content-Type: application/json
```

Solicitud de ejemplo:

```json
{
  "nombre": "Ana Pérez",
  "telefono": "+56 9 9876 5432"
}
```

```bash
curl -X PUT http://localhost:5210/api/contactos/1 \
  -H "Content-Type: application/json" \
  -d '{"nombre":"Ana Pérez","telefono":"+56 9 9876 5432"}'
```

Respuesta de ejemplo (`200 OK`):

```json
{
  "id": 1,
  "nombre": "Ana Pérez",
  "telefono": "+56 9 9876 5432"
}
```

## Códigos de estado y reglas

| Código | Cuándo se usa |
| --- | --- |
| `200 OK` | Una consulta o actualización se completó correctamente. |
| `201 Created` | Se creó un contacto. |
| `400 Bad Request` | `nombre` o `telefono` falta, es nulo, está vacío o contiene solo espacios. |
| `404 Not Found` | No existe un contacto para el ID solicitado. |
| `409 Conflict` | El teléfono solicitado ya pertenece a otro contacto. |

Los errores controlados tienen el formato:

```json
{
  "message": "El teléfono es obligatorio."
}
```

El teléfono debe ser único, sin diferenciar mayúsculas y minúsculas. Al actualizar,
un contacto puede conservar su propio teléfono, pero no puede usar el de otro
contacto.

## Almacenamiento y concurrencia

Los contactos se guardan exclusivamente en memoria. Al detener o reiniciar la
API, todos los datos se pierden.

El repositorio usa una colección concurrente para lecturas y un bloqueo de
escritura para crear o actualizar. En esas operaciones, la validación final de
unicidad del teléfono y la escritura se realizan juntas, evitando que solicitudes
simultáneas asignen el mismo teléfono a dos contactos distintos.

## Estructura del proyecto

```text
src/
  ContactosApi/
    Common/        Resultados y tipos de error
    Controllers/   Endpoints HTTP
    Domain/        Modelo Contacto
    DTOs/          Solicitudes y respuestas HTTP
    Middleware/    Manejo global de errores inesperados
    Repositories/  Almacenamiento en memoria
    Services/      Reglas de negocio
tests/
  ContactosApi.Tests/
    Services/      Pruebas unitarias de servicios
    ContactosApiIntegrationTests.cs
```
