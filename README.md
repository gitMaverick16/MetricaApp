# MetricaApp

Aplicacion fullstack para autenticacion y gestion de pedidos.

## Stack

- Backend: .NET 8 Web API + EF Core + SQL Server + JWT
- Frontend: React + Vite + TypeScript
- Infra local: Docker + Docker Compose

## Ejecutar con Docker (recomendado para reviewer)

### Requisitos

- Docker Desktop

### Levantar todo

```bash
docker compose up --build -d
```

Servicios publicados:

- Frontend: http://localhost:3000
- Backend API: http://localhost:7203
- SQL Server: localhost,1433

### Detener

```bash
docker compose down
```

Para eliminar tambien el volumen de SQL Server:

```bash
docker compose down -v
```

## Credenciales de prueba (seed)

Usuarios que se crean al iniciar la API:

- Admin: `demo@demo.com` / `Password123!`
- User: `limited@demo.com` / `Password123!`

## Endpoints principales

- `POST /auth/login`
- `GET /api/pedidos`
- `GET /api/pedidos/{id}`
- `POST /api/pedidos`
- `PUT /api/pedidos/{id}`
- `DELETE /api/pedidos/{id}`

## Notas de Docker

- La API aplica migraciones automaticamente al iniciar.
- El frontend se construye con `VITE_API_URL=http://localhost:7203`.
- Si quieres cambiar la clave de `sa`, define `SA_PASSWORD` en tu entorno antes de `docker compose up`.
