# CAI Grupo 27

Trabajo practico de Construccion de Aplicaciones Informaticas.

## Integrantes

- Luciano Armas
- Christian Schuon
- Agustin Acosta

## Proyecto

La idea del proyecto es armar un e-commerce en C# y .NET usando microservicios.

Por ahora empezamos con `Products.API`, tomando como base la MiniApi que paso la catedra. La adaptamos para manejar productos, stock y categorias.

## Microservicios previstos

| Servicio | Descripcion | Estado |
| --- | --- | --- |
| Products.API | Gestion de productos, stock y categorias | Base funcional |
| Users.API | Registro, login y bloqueo de usuarios | Pendiente |
| Orders.API | Creacion y consulta de ordenes | Pendiente |
| Cart.API | Gestion del carrito de compras | Pendiente |
| Notifications.API | Registro y simulacion de notificaciones | Pendiente |

## Arquitectura base

La estructura sigue la base que paso el profesor:

- `Program.cs` deja la configuracion principal.
- `Extensions` separa servicios, middleware, endpoints y logging.
- `Data` contiene la inicializacion de SQLite y el repositorio.
- `Models` contiene los modelos y requests de productos.
- `HealthChecks` permite revisar si la API y la base estan funcionando.

Tambien dejamos Swagger para probar la API desde el navegador y un middleware de auditoria para las operaciones que modifican datos.

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

