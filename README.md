# PruebaCodex API

Este proyecto contiene una API REST creada con .NET 8 para administrar usuarios.

## Endpoints principales

- `POST /api/token`: recibe `clientId` y `clientSecret` y devuelve un JWT válido.
- `POST /api/login`: solicita `user` y `password` y responde si la operación fue exitosa.
- `POST /api/usuarios`: crea un usuario (requiere token válido).
- `GET /api/usuarios`: obtiene todos los usuarios registrados (requiere token válido).

La API utiliza autenticación OAuth2 basada en JWT para proteger los endpoints.

Para iniciar la API:

```bash
cd src/Api
# dotnet run
```

También puedes abrir `PruebaCodex.sln` con Visual Studio para compilar y ejecutar el proyecto.

Swagger está habilitado en `/swagger`.
