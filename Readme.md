# 📸 Photos Gateway API

API RESTful desarrollada en **ASP.NET Core 8.0** que actúa como Gateway para consumir la API externa de [JSONPlaceholder](https://jsonplaceholder.typicode.com/), implementando un CRUD completo con autenticación JWT y almacenamiento en memoria.

## 🏗️ Arquitectura del Proyecto

Apiapio/
├── Controllers/ # Controladores RESTful
│ ├── AlbumsController.cs
│ ├── AuthController.cs
│ ├── PhotosController.cs
│ └── UsersController.cs
├── Middleware/ # Middleware personalizado
│ └── ExceptionHandlingMiddleware.cs
├── Models/ # DTOs y modelos de datos
│ ├── Auth/
│ │ ├── AuthResponse.cs
│ │ ├── LoginRequest.cs
│ │ ├── RegisterRequest.cs
│ │ └── User.cs
│ ├── AddressDto.cs
│ ├── AlbumDto.cs
│ ├── CompanyDto.cs
│ ├── GeoDto.cs
│ ├── PhotoDto.cs
│ └── UserDto.cs
├── Repositories/ # Capa de acceso a datos
│ ├── IAlbumRepository.cs
│ ├── IPhotoRepository.cs
│ ├── IUserRepository.cs
│ ├── JsonPlaceholderAlbumRepository.cs
│ ├── JsonPlaceholderRepository.cs
│ └── JsonPlaceholderUserRepository.cs
├── Services/ # Lógica de negocio
│ ├── IAlbumService.cs
│ ├── IAuthService.cs
│ ├── IPhotoService.cs
│ ├── IUserService.cs
│ ├── AlbumService.cs
│ ├── AuthService.cs
│ ├── PhotoService.cs
│ ├── UserService.cs
│ ├── InMemoryAlbumCache.cs
│ ├── InMemoryPhotoCache.cs
│ ├── InMemoryUserCache.cs
│ └── InMemoryUserStore.cs
├── Properties/
├── obj/
├── Apiapio.csproj
├── Apiapio.http
├── appsettings.json
├── appsettings.Development.json
└── Program.cs

text

## 🚀 Características

### ✨ Funcionalidades Principales

- **CRUD Completo**: Photos, Users y Albums
- **Autenticación JWT**: Login, Register y gestión de tokens
- **Autorización por Roles**: User y Admin
- **Cache en Memoria**: Persistencia local de datos creados
- **Manejo Global de Errores**: Middleware personalizado
- **Documentación Swagger**: Interfaz interactiva con soporte JWT
- **Consumo de API Externa**: Integración con JSONPlaceholder
- **Validación de Datos**: Data Annotations en todos los DTOs

### 🔒 Niveles de Seguridad

| Operación | Autenticación | Rol Requerido |
|-----------|--------------|---------------|
| `GET` (Lectura) | ✅ Requerida | User o Admin |
| `POST` (Crear) | ✅ Requerida | User o Admin |
| `PUT` (Actualizar) | ✅ Requerida | User o Admin |
| `DELETE` (Eliminar) | ✅ Requerida | **Solo Admin** |

## 📋 Endpoints Disponibles

### 🔐 Autenticación (`/api/auth`)

| Método | Endpoint | Descripción | Auth |
|--------|----------|-------------|------|
| `POST` | `/api/auth/login` | Iniciar sesión | ❌ |
| `POST` | `/api/auth/register` | Registrar usuario | ❌ |
| `GET` | `/api/auth/profile` | Obtener perfil | ✅ |
| `GET` | `/api/auth/admin-only` | Endpoint Admin | ✅ Admin |

### 📷 Photos (`/api/photos`)

| Método | Endpoint | Descripción | Auth |
|--------|----------|-------------|------|
| `GET` | `/api/photos` | Listar todas las fotos | ✅ |
| `GET` | `/api/photos/{id}` | Obtener foto por ID | ✅ |
| `POST` | `/api/photos` | Crear nueva foto | ✅ |
| `PUT` | `/api/photos/{id}` | Actualizar foto | ✅ |
| `DELETE` | `/api/photos/{id}` | Eliminar foto | ✅ Admin |

### 👥 Users (`/api/users`)

| Método | Endpoint | Descripción | Auth |
|--------|----------|-------------|------|
| `GET` | `/api/users` | Listar todos los usuarios | ✅ |
| `GET` | `/api/users/{id}` | Obtener usuario por ID | ✅ |
| `POST` | `/api/users` | Crear nuevo usuario | ✅ |
| `PUT` | `/api/users/{id}` | Actualizar usuario | ✅ |
| `DELETE` | `/api/users/{id}` | Eliminar usuario | ✅ Admin |

### 📚 Albums (`/api/albums`)

| Método | Endpoint | Descripción | Auth |
|--------|----------|-------------|------|
| `GET` | `/api/albums` | Listar todos los álbumes | ✅ |
| `GET` | `/api/albums/{id}` | Obtener álbum por ID | ✅ |
| `GET` | `/api/albums/user/{userId}` | Álbumes por usuario | ✅ |
| `POST` | `/api/albums` | Crear nuevo álbum | ✅ |
| `PUT` | `/api/albums/{id}` | Actualizar álbum | ✅ |
| `DELETE` | `/api/albums/{id}` | Eliminar álbum | ✅ Admin |

## 🛠️ Tecnologías Utilizadas

- **.NET 8.0** - Framework principal
- **ASP.NET Core Web API** - Motor de la API
- **JWT (JSON Web Tokens)** - Autenticación y autorización
- **HttpClient Factory** - Consumo de API externa
- **Swagger/OpenAPI** - Documentación interactiva
- **System.IdentityModel.Tokens.Jwt** - Gestión de tokens
- **Microsoft.AspNetCore.Authentication.JwtBearer** - Middleware JWT

## 📦 Dependencias (NuGet)

```xml
<PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" Version="8.0.*" />
<PackageReference Include="System.IdentityModel.Tokens.Jwt" Version="8.0.*" />
<PackageReference Include="Swashbuckle.AspNetCore" Version="6.*" />
⚙️ Configuración
appsettings.json
json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ExternalApis": {
    "JsonPlaceholder": {
      "BaseUrl": "https://jsonplaceholder.typicode.com/",
      "Timeout": 30
    }
  },
  "JwtSettings": {
    "SecretKey": "TuClaveSecretaSuperSeguraDeAlMenos32Caracteres!",
    "Issuer": "PhotosGatewayAPI",
    "Audience": "PhotosGatewayAPIUsers",
    "ExpirationMinutes": 60
  }
}
🚀 Instalación y Ejecución
Prerrequisitos
.NET 8.0 SDK o superior

Visual Studio 2022 / VS Code / Rider

Pasos
Clonar el repositorio

bash
git clone <tu-repositorio>
cd Apiapio
Restaurar dependencias

bash
dotnet restore
Compilar el proyecto

bash
dotnet build
Ejecutar la aplicación

bash
dotnet run
Abrir Swagger

text
https://localhost:7xxx/swagger
🧪 Testing en Swagger
1. Login como Admin
Endpoint: POST /api/auth/login

Request:

json
{
  "username": "admin",
  "password": "Admin123!"
}
Response (200 OK):

json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "username": "admin",
  "email": "admin@photosgateway.com",
  "expiresAt": "2026-01-20T19:50:00Z"
}
2. Autorizar en Swagger
Copia el token de la respuesta

Click en el botón "Authorize" 🔓

Pega el token (sin "Bearer")

Click en "Authorize" → "Close"

3. Crear una Foto
Endpoint: POST /api/photos

Request:

json
{
  "albumId": 1,
  "title": "Mi foto de prueba",
  "url": "https://via.placeholder.com/600/test",
  "thumbnailUrl": "https://via.placeholder.com/150/test"
}
4. Registrar Usuario Normal
Endpoint: POST /api/auth/register

Request:

json
{
  "username": "testuser",
  "email": "test@example.com",
  "password": "Test123!",
  "confirmPassword": "Test123!"
}
👤 Usuarios por Defecto
Username	Password	Role	Email
admin	Admin123!	Admin	admin@photosgateway.com
🏛️ Patrones de Diseño Implementados
Repository Pattern
Abstracción de la capa de acceso a datos separando la lógica de persistencia.

text
IPhotoRepository → JsonPlaceholderRepository
IUserRepository → JsonPlaceholderUserRepository
IAlbumRepository → JsonPlaceholderAlbumRepository
Service Layer Pattern
Capa intermedia con la lógica de negocio entre controladores y repositorios.

text
Controller → Service → Repository → API Externa
Dependency Injection
Inyección de dependencias en toda la aplicación para facilitar testing y mantenibilidad.

HttpClient Factory Pattern
Gestión eficiente de HttpClient evitando socket exhaustion.

🔄 Flujo de Datos
text
Cliente (Swagger/Postman)
    ↓
JWT Authentication Middleware
    ↓
Controller (PhotosController, UsersController, AlbumsController)
    ↓
Service (PhotoService, UserService, AlbumService)
    ↓
Repository (JsonPlaceholderRepository)
    ↓
HttpClient → JSONPlaceholder API Externa
    ↓
InMemoryCache (para datos creados localmente)
🔐 Sistema de Cache en Memoria
La API implementa un sistema de cache para simular persistencia de datos:

InMemoryPhotoCache: IDs >= 5001

InMemoryAlbumCache: IDs >= 2001

InMemoryUserCache: IDs >= 1001

InMemoryUserStore: Usuarios de autenticación

Los datos creados localmente se combinan con los de la API externa en las consultas GET.

📝 Commits Realizados
bash
# Estructura inicial
git commit -m "feat: initialize .NET Web API project with minimal configuration"
git commit -m "feat: add Photo DTOs for external API integration"

# Middleware y manejo de errores
git commit -m "feat: agregar middleware global para manejo de excepciones con respuestas estructuradas"

# Repository Pattern
git commit -m "feat: implementar patrón Repository para abstracción de acceso a datos externos"

# Service Layer
git commit -m "feat: agregar capa de servicios con lógica de negocio para gestión de fotos"

# Controllers
git commit -m "feat: crear controlador RESTful con endpoints para gestión de fotos"

# CRUD completo
git commit -m "feat: implementar operaciones CRUD completas en Repository (POST, PUT, DELETE)"

# Cache en memoria
git commit -m "feat: agregar cache en memoria para persistencia local de fotos"

# Users y Albums
git commit -m "feat: agregar DTOs de User y Album con validaciones"
git commit -m "feat: implementar repositorios para Users y Albums con cache integrado"
git commit -m "feat: crear controladores RESTful completos para Users y Albums con CRUD"

# Autenticación JWT
git commit -m "feat: agregar modelos de autenticación (Login, Register, User, AuthResponse)"
git commit -m "feat: implementar servicio de autenticación con JWT y store en memoria"
git commit -m "feat: crear controlador de autenticación con endpoints de login, register y profile"

# Seguridad
git commit -m "security: proteger endpoints de Photos con JWT (Admin para DELETE)"
git commit -m "security: proteger endpoints de Users con JWT (Admin para DELETE)"
git commit -m "security: proteger endpoints de Albums con JWT (Admin para DELETE)"

# Configuración
git commit -m "config: configurar autenticación JWT con Bearer token y soporte en Swagger"
git commit -m "config: registrar servicios y repositorios de Users y Albums en contenedor DI"
🎯 Características Avanzadas
✅ CORS habilitado para consumo desde clientes web

✅ Validación automática con DataAnnotations

✅ Logging estructurado en todos los servicios

✅ Manejo de errores centralizado con middleware

✅ Documentación OpenAPI/Swagger con autenticación JWT

✅ Timeout configurable para llamadas HTTP

✅ Claims personalizados en tokens JWT

✅ Roles y políticas de autorización

📚 Recursos Externos
JSONPlaceholder - API externa consumida

JWT.io - Debug de tokens JWT

Swagger UI - Documentación interactiva

👨‍💻 Autor
Desarrollado como proyecto de aprendizaje de ASP.NET Core Web API con integración de servicios externos y autenticación JWT.

📄 Licencia
Este proyecto es de código abierto para fines educativos.

Nota: Esta API es un proyecto de demostración. En producción, considera usar:

Base de datos real (SQL Server, PostgreSQL)

Hashing más robusto (BCrypt, Argon2)

Secrets Manager para claves JWT

Rate limiting y throttling

Logging persistente (Serilog, ELK Stack)

Health checks y métricas