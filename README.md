# CAI Grupo 27

Trabajo practico de Construccion de Aplicaciones Informaticas.

## Integrantes

- Luciano Armas
- Christian Schuon
- Agustin Acosta

## Proyecto

Sistema e-commerce desarrollado en C# y .NET, organizado como una arquitectura de microservicios.

## Microservicios previstos

| Servicio | Descripcion | Estado |
| --- | --- | --- |
| Products.API | Gestion de productos, stock y categorias | Base funcional |
| Users.API | Registro, login y bloqueo de usuarios | Pendiente |
| Orders.API | Creacion y consulta de ordenes | Pendiente |
| Cart.API | Gestion del carrito de compras | Pendiente |
| Notifications.API | Registro y simulacion de notificaciones | Pendiente |

## Arquitectura base

El proyecto toma como referencia la arquitectura MiniApi provista por la catedra, separando configuracion, endpoints, middleware, acceso a datos, health checks y logging.

Products.API incluye:

- Endpoints REST para listar, consultar, crear, modificar y eliminar productos.
- Persistencia local con SQLite.
- Swagger UI para probar la API desde el navegador.
- Health checks para validar estado de la API y la base SQLite.
- Middleware de auditoria para operaciones POST, PUT y DELETE.
- Logging con Serilog en consola y archivo.

## Ejecucion

Restaurar y compilar la solucion:

```bash
dotnet restore ECommerce.sln --ignore-failed-sources
dotnet build ECommerce.sln --no-restore
```

Ejecutar Products.API:

```bash
dotnet run --project src/Products.API/Products.API.csproj
```

Endpoints principales:

| Metodo | Ruta | Descripcion |
| --- | --- | --- |
| GET | /products | Lista todos los productos |
| GET | /products/{id} | Consulta un producto por id |
| POST | /products | Crea un producto |
| PUT | /products/{id} | Actualiza un producto |
| DELETE | /products/{id} | Elimina un producto |
| GET | /health | Estado tecnico de la API |
| GET | /health-ui | Panel visual de health checks |

## Documentacion pendiente

- Tabla de puertos por microservicio.
- Tabla de codigos de error por API.
- Diagrama de arquitectura.
- Capturas de Swagger UI con respuestas de exito y error.

