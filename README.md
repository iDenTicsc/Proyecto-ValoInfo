# ValoInfo Backend

REST API para ValoInfo, una plataforma de información sobre agentes de Valorant en español. Construida con ASP.NET Core 10.0 siguiendo Clean Architecture, con Google Firestore como base de datos y soporte de versionamiento de API.

---

## Tabla de contenidos

- [Requisitos](#requisitos)
- [Estructura del proyecto](#estructura-del-proyecto)
- [Configuración](#configuración)
- [Ejecución local](#ejecución-local)
- [Ejecución con Docker](#ejecución-con-docker)
- [Endpoints de la API](#endpoints-de-la-api)
- [Estructura de respuesta](#estructura-de-respuesta)
- [Modelos de datos](#modelos-de-datos)
- [Pruebas](#pruebas)
- [Notas de arquitectura](#notas-de-arquitectura)

---

## Requisitos

- [.NET 10.0 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- Proyecto de Google Firebase con Firestore habilitado (colección `Agentes`)
- Credenciales de servicio de Firebase (archivo JSON de cuenta de servicio)
- Docker (opcional, para despliegue en contenedor)

---

## Estructura del proyecto

```
Backend/
├── src/
│   ├── ValoInfo.Api/            # Capa de presentación: controladores, middleware, Program.cs
│   ├── ValoInfo.Application/    # Capa de aplicación: interfaces, DTOs, mappers
│   ├── ValoInfo.Domain/         # Capa de dominio: entidades, ApiResponse<T>
│   └── ValoInfo.Infrastructure/ # Capa de infraestructura: repositorios Firestore
├── tests/
│   └── ValoInfo.Tests/          # Pruebas unitarias (xUnit + Moq + FluentAssertions)
├── Dockerfile
└── Backend.sln
```

---

## Configuración

### Variables de entorno

| Variable                        | Descripción                                                                 | Requerida |
|---------------------------------|-----------------------------------------------------------------------------|-----------|
| `FIRESTORE_CREDENTIALS_BASE64`  | JSON de la cuenta de servicio de Firebase codificado en Base64              | Si        |
| `PORT`                          | Puerto en el que escucha la API (default: `8080`)                           | No        |

Para obtener el valor de `FIRESTORE_CREDENTIALS_BASE64`, codifica el archivo JSON de credenciales de Firebase:

```bash
# Linux / macOS
base64 -i firebase-credentials.json

# Windows (PowerShell)
[Convert]::ToBase64String([IO.File]::ReadAllBytes("firebase-credentials.json"))
```

### Ejecución local con archivo .env

En desarrollo la aplicación carga automáticamente un archivo `.env` en la raíz del proyecto. Crea el archivo con el siguiente contenido:

```env
FIRESTORE_CREDENTIALS_BASE64=<tu_json_en_base64>
PORT=8080
```

### CORS

Los orígenes permitidos se configuran en `src/ValoInfo.Api/appsettings.json`:

```json
{
  "Cors": {
    "AllowedOrigins": [
      "http://localhost:3000",
      "https://tu-frontend.com"
    ]
  }
}
```

---

## Ejecución local

```bash
# Restaurar dependencias y compilar
dotnet build Backend.sln

# Ejecutar la API
dotnet run --project src/ValoInfo.Api/ValoInfo.Api.csproj
```

La API estara disponible en `http://localhost:8080`.

En modo desarrollo, Swagger UI estará disponible en `http://localhost:8080/swagger`.

---

## Ejecución con Docker

```bash
# Construir la imagen
docker build -t valoinfo-backend .

# Ejecutar el contenedor
docker run -p 8080:8080 \
  -e FIRESTORE_CREDENTIALS_BASE64="<tu_json_en_base64>" \
  -e PORT=8080 \
  valoinfo-backend
```

El Dockerfile usa una build multi-stage:
1. **build** — compila y publica la aplicación con el SDK de .NET 10.
2. **final** — imagen de runtime mínima basada en `aspnet:10.0-preview`.

---

## Endpoints de la API

La versión de API se especifica en la URL. La versión por defecto es `v1.0`.

### GET `/api/v1.0/agentes`

Retorna la lista completa de agentes disponibles.

**Respuesta exitosa — 200 OK**

```json
{
  "success": true,
  "data": [
    {
      "nombre": "Phoenix",
      "rol": "Duelista",
      "biografia": "...",
      "imagen": "https://...",
      "habilidades": [...]
    }
  ],
  "message": null,
  "statusCode": 200
}
```

---

### GET `/api/v1.0/agentes/{id}`

Retorna la información de un agente específico por su ID (nombre en Firestore).

**Parámetros de ruta**

| Parámetro | Tipo   | Descripción                        |
|-----------|--------|------------------------------------|
| `id`      | string | Identificador del agente en Firestore |

**Respuesta exitosa — 200 OK**

```json
{
  "success": true,
  "data": {
    "nombre": "Phoenix",
    "rol": "Duelista",
    "biografia": "...",
    "imagen": "https://...",
    "habilidades": [
      {
        "keybind": "C",
        "nombreHabilidad": "Blaze",
        "descripcion": "...",
        "logo": "https://...",
        "video": "https://..."
      }
    ]
  },
  "message": null,
  "statusCode": 200
}
```

**Respuesta de error — 404 Not Found**

```json
{
  "success": false,
  "data": null,
  "message": "No se encontró un agente con el ID proporcionado.",
  "statusCode": 404
}
```

**Respuesta de error — 400 Bad Request**

```json
{
  "success": false,
  "data": null,
  "message": "El ID no puede estar vacío.",
  "statusCode": 400
}
```

---

## Estructura de respuesta

Todos los endpoints retornan un objeto `ApiResponse<T>` con la siguiente forma:

```json
{
  "success": true | false,
  "data": <T> | null,
  "message": "string descriptivo" | null,
  "statusCode": 200 | 400 | 404 | 500
}
```

Los errores no controlados son interceptados por `ExceptionMiddleware` y retornan siempre un `500` con el mismo formato, sin exponer detalles internos al cliente.

---

## Modelos de datos

### Agente (entidad de dominio — colección Firestore `Agentes`)

| Campo          | Tipo              | Descripción                        |
|----------------|-------------------|------------------------------------|
| `id`           | string            | Identificador del documento        |
| `nombre`       | string            | Nombre del agente                  |
| `rol`          | string            | Rol (ej. Duelista, Controlador)    |
| `biografia`    | string            | Descripción del agente             |
| `imagen`       | string            | URL de la imagen del agente        |
| `habilidades`  | List\<Habilidad\> | Lista de habilidades del agente    |

### Habilidad

| Campo              | Tipo   | Descripción                                 |
|--------------------|--------|---------------------------------------------|
| `keybind`          | string | Tecla de activación (ej. `C`, `Q`, `E`, `X`) |
| `nombre_habilidad` | string | Nombre de la habilidad                      |
| `descripcion`      | string | Descripción de la habilidad                 |
| `logo`             | string | URL del ícono de la habilidad               |
| `video`            | string | URL del video de demostración               |

> El DTO de respuesta (`AgenteResponse`) excluye el campo `id` por diseño; no se expone en la API.

---

## Pruebas

El proyecto incluye 11 pruebas unitarias que cubren el controlador y el mapper.

```bash
dotnet test tests/ValoInfo.Tests/ValoInfo.Tests.csproj
```

### Cobertura de pruebas

| Clase probada       | Casos cubiertos                                                    |
|---------------------|--------------------------------------------------------------------|
| `AgentsController`  | GET all con datos, GET all vacío, GET all con excepción del repo   |
|                     | GET by id encontrado, GET by id no encontrado, GET by id vacío     |
| `AgentMapper`       | Mapeo completo de campos, mapeo de habilidades, lista de habilidades vacía, ausencia del campo `Id` en el DTO |

**Stack de pruebas**

- [xUnit](https://xunit.net/) — framework de pruebas
- [Moq](https://github.com/devlooped/moq) — mocking de dependencias
- [FluentAssertions](https://fluentassertions.com/) — aserciones expresivas

---

## Notas de arquitectura

El proyecto sigue los principios de **Clean Architecture** con separación estricta por capas:

- **Domain** — entidades (`Agente`, `Habilidad`) y tipos comunes (`ApiResponse<T>`). Sin dependencias externas.
- **Application** — contratos (`IAgentRepository`), DTOs de respuesta (`AgenteResponse`) y mappers (`AgentMapper`). Sin acceso a infraestructura.
- **Infrastructure** — implementación concreta de `IAgentRepository` usando `Google.Cloud.Firestore` (`FirestoreAgentRepository`).
- **Api** — controladores MVC, configuración de versionamiento, middleware de excepciones y entrada del programa (`Program.cs`).

El versionamiento de API se realiza por segmento de URL (`/api/v1.0/...`) usando `Microsoft.AspNetCore.Mvc.Versioning`. La versión `2.0` está declarada en el controlador pero aún no tiene endpoints propios asignados.
