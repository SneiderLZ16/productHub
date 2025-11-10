
# 🛍️ ProductHub API

API REST para manejo de **Usuarios** y **Productos**, desarrollada con:

- 🟦 **ASP.NET Core 8**
- 🏛️ **Clean Architecture (Domain / Application / Infrastructure / Api)**
- 🗄️ **MySQL (Aiven Cloud)**
- 🔐 **Autenticación JWT**
- 🐳 **Docker + Render Deploy**
- ✅ Pruebas con **xUnit**

---

## 📌 Estructura del Proyecto

```
productHub/
├── productHub.Api                --> Capa de presentación (Controllers, Program.cs)
├── productHub.Application        --> Casos de uso, servicios, DTOs, Interfaces
├── productHub.Domain             --> Entidades y contratos del dominio
├── productHub.Infrastructure     --> Persistencia EF Core + JWT + Repositorios
```

---

## 🚀 Endpoints principales

| Método | Endpoint                | Auth requerida | Descripción                           |
|--------|------------------------|---------------|---------------------------------------|
| POST   | `/api/auth/register`   | ❌            | Registrar usuario                     |
| POST   | `/api/auth/login`      | ❌            | Autenticar usuario y generar JWT      |
| GET    | `/api/products`        | ✅            | Obtener lista de productos            |
| POST   | `/api/products`        | ✅            | Crear producto                        |

> Para usar endpoints protegidos debes enviar el token generado por `/auth/login`
> en los headers:

```
Authorization: Bearer {token}
```

---

## 🔧 Variables de entorno necesarias

Estas variables deben configurarse en Render o en tu entorno local (`.env`):

| Variable | Descripción |
|----------|-------------|
| `ConnectionStrings__DefaultConnection` | Cadena de conexión MySQL (Aiven) |
| `JWT_SECRET` | Llave secreta para firmar JWT |
| `JWT_ISSUER` | (Opcional) Identificador del issuer |
| `JWT_AUDIENCE` | (Opcional) Identificador del audience |

Ejemplo:

```
ConnectionStrings__DefaultConnection=server=xxxxx;aivencloud.com;port=12345;database=defaultdb;user=avnadmin;password=MY_PASS;sslmode=Required;
JWT_SECRET=WZllun8Kv5G8hL3xBKPqshPMgFYStU8a
JWT_ISSUER=ProductHub
JWT_AUDIENCE=ProductHubUsers
```

---

## ▶️ Ejecutar el proyecto localmente

1. Restaurar dependencias
```sh
dotnet restore
```

2. Aplicar migraciones
```sh
dotnet ef database update  --project productHub.Infrastructure  --startup-project productHub.Api
```

3. Ejecutar
```sh
dotnet run --project productHub.Api
```

Luego abre:

👉 http://localhost:8080/swagger

---

## 🐳 Docker

### Construir imagen
```sh
docker build -t producthub-api .
```

### Correr contenedor
```sh
docker run -d -p 8080:8080 --env-file .env producthub-api
```

---

## ⚙️ CI/CD Render

Render detecta automáticamente el `Dockerfile`.

Solo debes agregar las **ENV VARS** en *Environment → Variables*.

---

## ✅ Pruebas Unitarias (xUnit)

Ubicadas en `productHub.Application.Tests/`

Ejecutar pruebas:
```sh
dotnet test
```

---

## ✨ Tecnologías utilizadas

| Tecnología | Uso |
|------------|------|
| ASP.NET Core 8 | API + Swagger |
| MySQL / Aiven | Base de datos |
| Entity Framework Core | ORM |
| JWT | Autenticación |
| Docker | Contenerización |
| Render | Hosting |
| xUnit | Testing |

---

## 👨‍💻 Autor

**Sneider Londoño**  
📌 Backend Developer – .NET / Node / React

---
