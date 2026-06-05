# API de Contactos

API REST desarrollada en .NET 10 para gestionar contactos utilizando almacenamiento en memoria, sin base de datos.

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

- .NET 10
- ASP.NET Core Web API
- Swagger / OpenAPI
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