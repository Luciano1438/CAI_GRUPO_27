# CAI Grupo 27

Trabajo practico de Construccion de Aplicaciones Informaticas.

## Integrantes

- Luciano Armas
- Christian Schuon
- Agustin Acosta

## Idea del proyecto

Vamos a armar un e-commerce en C# y .NET usando una arquitectura de microservicios.

La base que paso la catedra esta copiada en la carpeta `MiniApi`. La dejamos en el repo para tener la referencia original y usarla como punto de partida.

Tambien empezamos una primera adaptacion en `src/Products.API`, orientada al manejo de productos, stock y categorias.

## Estado actual

Por ahora el repo tiene:

- La base original de la catedra en `MiniApi`.
- Una solucion del proyecto en `ECommerce.sln`.
- El microservicio `Products.API` con endpoints para productos.
- Persistencia local con SQLite.
- Swagger, health checks, logging y middleware de auditoria, siguiendo la estructura de la MiniApi.

Microservicios pensados para el TP:

- `Products.API`: productos, stock y categorias.
- `Users.API`: usuarios, login y bloqueo.
- `Orders.API`: ordenes.
- `Cart.API`: carrito.
- `Notifications.API`: notificaciones.

## Como correr Products.API

Restaurar y compilar:

```bash
dotnet restore ECommerce.sln --ignore-failed-sources
dotnet build ECommerce.sln --no-restore
```

Ejecutar:

```bash
dotnet run --project src/Products.API/Products.API.csproj
```

Endpoints principales:

- `GET /products`
- `GET /products/{id}`
- `POST /products`
- `PUT /products/{id}`
- `DELETE /products/{id}`
- `GET /health`
- `GET /health-ui`

## Pendiente

- Definir bien que hace cada microservicio.
- Agregar los otros proyectos al repo.
- Documentar puertos.
- Agregar codigos de error.
- Subir capturas de Swagger cuando vayamos probando.

