# MetricaApp

Aplicacion fullstack para autenticacion de usuarios y gestion de pedidos.

---

## 1) Ejecucion rapida (primero para reviewer)

La forma recomendada de probar el proyecto es con Docker Compose.

### Requisitos

- Docker Desktop instalado y en ejecucion.

### Levantar todo (DB + API + Frontend)

```bash
docker compose up --build -d
```

### URLs

- Frontend: [http://localhost:3000](http://localhost:3000)
- Backend API: [http://localhost:7203](http://localhost:7203)
- SQL Server: `localhost,1433`

### Detener servicios

```bash
docker compose down
```

Para limpiar tambien el volumen de SQL Server:

```bash
docker compose down -v
```

---

## 2) Como probar la aplicacion

1. Abre [http://localhost:3000](http://localhost:3000).
2. Inicia sesion con uno de los usuarios seed.
3. Valida comportamiento segun rol:
  - `Admin`: puede listar, crear, editar y eliminar pedidos.
  - `User`: al entrar al modulo de pedidos recibe `403 Forbidden` desde la API.
4. Verifica CRUD de pedidos con el usuario Admin.

---

## 3) Usuarios de prueba y roles

Usuarios creados automaticamente por seed:

- **Admin**
  - Email: `demo@demo.com`
  - Password: `Password123!`
  - Permisos: acceso completo a `/api/pedidos` (CRUD).
- **User**
  - Email: `limited@demo.com`
  - Password: `Password123!`
  - Permisos: autenticado, pero sin acceso al CRUD de pedidos (API devuelve `403`).

Implementacion de autorizacion por rol en backend:

- Controlador protegido con `[Authorize(Roles = AppRoles.Admin)]` en `OrdersController`.

---

## 4) Endpoints principales

### Autenticacion

- `POST /auth/login`

Request:

```json
{
  "email": "demo@demo.com",
  "password": "Password123!"
}
```

Response:

```json
{
  "token": "jwt_token",
  "expiresIn": 3600
}
```

### Pedidos (requiere JWT y rol Admin)

- `GET /api/pedidos`
- `GET /api/pedidos/{id}`
- `POST /api/pedidos`
- `PUT /api/pedidos/{id}`
- `DELETE /api/pedidos/{id}`

---

## 5) Reglas de negocio cubiertas

- No se permite crear/editar pedidos con `total <= 0`.
- `numeroPedido` es unico (validacion en aplicacion + restriccion unica en DB).
- Solo usuarios autenticados y autorizados pueden acceder al CRUD.
- Eliminacion logica implementada (`IsDeleted`, `DeletedAt`, query filter global).

---

## 6) Arquitectura y decisiones tecnicas

### Stack

- Backend: .NET 8, Web API, EF Core, SQL Server, JWT, MediatR.
- Frontend: React + Vite + TypeScript.
- Observabilidad: Serilog (logs estructurados).

### Estructura backend (Clean Architecture)

- `Backend/OrdersApp/src/OrdersApp.Api`: controllers, contratos HTTP, manejo global de excepciones.
- `Backend/OrdersApp/src/OrdersApp.Application`: casos de uso, comandos/queries, interfaces.
- `Backend/OrdersApp/src/OrdersApp.Domain`: entidades y reglas de dominio.
- `Backend/OrdersApp/src/OrdersApp.Infrastructure`: EF Core, repositorios, auth, seed, configuraciones.

### Seguridad

- JWT Bearer con expiracion configurable.
- Claims de usuario y rol en token.
- Middleware de autenticacion/autorizacion habilitado.
- Restriccion por rol en endpoints de pedidos.

### Persistencia

- SQL Server con EF Core.
- Migraciones aplicadas automaticamente al iniciar la API.
- Seed inicial de usuarios y pedidos en entorno de desarrollo.

---

## 7) Testing (xUnit)

Se agrego suite basica de unit testing en:

- `Backend/OrdersApp/test/OrdersApp.UnitTests`

Cobertura incluida:

- Dominio (`Order`): validaciones y eliminacion logica.
- Aplicacion (`CreateOrderCommandHandler`): conflicto por duplicado y validacion de total.

Ejecutar tests:

```bash
dotnet test ./Backend/OrdersApp/test/OrdersApp.UnitTests/OrdersApp.UnitTests.csproj
```

---

## 8) Variables y configuracion

### Docker Compose

- Archivo: `docker-compose.yml`
- Variable opcional: `SA_PASSWORD` (puede definirse en entorno o en `.env`).
- Referencia: `.env.example`.

### Credenciales de base de datos (entorno local Docker)

Para conectarte desde una herramienta externa (SSMS, Azure Data Studio, DBeaver):

- Host: `localhost,1433`
- Usuario: `sa`
- Password por defecto: `StrongPassword!123` (o el valor que definas en `SA_PASSWORD`)
- Base de datos: `PedidosDB`

### Nota sobre connection string en debug local

Si levantas solo la DB en Docker y ejecutas backend manualmente, la password de la cadena de conexion del backend debe coincidir con la del contenedor SQL.

---

## 9) Puntos de mejora (backlog)

- Mover secretos sensibles fuera de `appsettings` a `User Secrets`/variables de entorno en todos los entornos.
- Agregar pruebas de integracion (API + DB) y mas cobertura de casos de aplicacion.
- Incluir coleccion de Postman/Bruno para pruebas manuales guiadas.
- Endurecer politicas de resiliencia de DB (retry explicito y timeouts configurados por escenario).
- Pipeline CI para build, test y analisis estatico.

