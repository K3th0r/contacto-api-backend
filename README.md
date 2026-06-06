# API de Contactos

API REST desarrollada en .NET 8 para gestionar contactos utilizando almacenamiento en memoria, sin base de datos.

## Objetivo

La solución permite:

- Obtener todos los contactos.
- Obtener un contacto por ID.
- Crear un contacto.
- Evitar contactos duplicados por teléfono.
- Responder con códigos HTTP adecuados.
- Mantener la información en memoria.
- Ejecutar la API localmente sin configuraciones adicionales.

## Tecnologías utilizadas

- .NET 8
- ASP.NET Core Web API
- Swagger
- xUnit
- FluentAssertions
- Microsoft.AspNetCore.Mvc.Testing

## Estructura del proyecto

```txt
ContactosApi/
│
├── src/
│   └── ContactosApi/
│       ├── Common/
│       ├── Controllers/
│       ├── Domain/
│       ├── DTOs/
│       ├── Middleware/
│       ├── Repositories/
│       ├── Services/
│       └── Program.cs
│
├── tests/
│   └── ContactosApi.Tests/
│       ├── Services/
│       └── ContactosApiIntegrationTests.cs
│
└── README.md
```
## Requisitos

- .NET 8 SDK o superior compatible con `net8.0`.

Para verificar la instalación:

```bash
dotnet --version
```

## Ejecución local

Clonar el repositorio:

```bash
git clone https://github.com/K3th0r/contacto-api-backend.git
cd contacto-api-backend
```
Restaurar dependencias:

```bash
dotnet restore
```
Ejecutar la API:

```bash
dotnet run --project src/ContactosApi/ContactosApi.csproj
```
La consola mostrará una URL similar a:

http://localhost:XXXX

Abrir Swagger en el navegador:

http://localhost:XXXX/swagger


## Endpoints disponibles

### Obtener todos los contactos

```http
GET /api/contactos
```

Usando curl

```bash
curl http://localhost:5189/api/contactos
```

Obtener contacto por ID

```http
GET /api/contactos/{id}
```

Ejemplo:

```bash
curl http://localhost:5189/api/contactos/1
```

Crear contacto

```http
POST /api/contactos
```
Body:

{
  "nombre": "Juan Perez",
  "telefono": "123456789"
}

Ejemplo usando curl:
```bash
curl -X POST http://localhost:5189/api/contactos ^
  -H "Content-Type: application/json" ^
  -d "{\"nombre\":\"Juan Perez\",\"telefono\":\"123456789\"}"
```
## Instrucciones para tests

Desde la raíz del repositorio:

```bash
dotnet test
```

La solución incluye pruebas unitarias y de integración.
