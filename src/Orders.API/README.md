# Orders.API

API de órdenes del proyecto CAI_GRUPO_27, desarrollada con ASP.NET Core
sobre .NET 10 y persistencia en SQLite.

## Funcionalidades

- Crear órdenes validando usuario, productos y stock.
- Agrupar productos repetidos y calcular el total con precios de Products.API.
- Guardar la orden y sus ítems en una transacción.
- Listar órdenes y filtrar por usuario.
- Consultar una orden por ID.
- Actualizar el estado respetando las transiciones permitidas.
- Devolver errores uniformes con código y correlación.
- Registrar logs estructurados.
- Documentar los endpoints mediante Swagger.

La validación de stock consulta Products.API; no descuenta ni reserva unidades.

## Ejecución local

Requiere el SDK de .NET 10.

Desde la raíz del repositorio, ejecutar cada comando en una terminal distinta:

```powershell
dotnet run --project src/Users.API/Users.API.csproj --launch-profile http
dotnet run --project src/Products.API/Products.API.csproj --launch-profile http
dotnet run --project src/Orders.API/Orders.API.csproj --launch-profile http
```

- Orders: http://localhost:5165
- Swagger: http://localhost:5165/swagger
- Users: http://localhost:5290
- Products: http://localhost:5208

Swagger está habilitado en el entorno Development.

## Configuración

En appsettings.json:

- ConnectionStrings:DefaultConnection: conexión a SQLite.
- Services:Users: dirección base de Users.API.
- Services:Products: dirección base de Products.API.

Al iniciar, DatabaseInitializer prepara las tablas necesarias.
La configuración local utiliza el archivo orders.db.

## Endpoints

| Método | Ruta | Función |
|---|---|---|
| GET | /api/orders | Listar órdenes |
| GET | /api/orders?usuarioId={id} | Filtrar por usuario |
| GET | /api/orders/{id} | Consultar una orden |
| POST | /api/orders | Crear una orden |
| PUT | /api/orders/{id}/status | Cambiar el estado |

## Crear una orden

Los identificadores deben corresponder a un usuario y un producto existentes.

```json
{
  "usuarioId": "GUID-DEL-USUARIO",
  "items": [
    {
      "productoId": "GUID-DEL-PRODUCTO",
      "cantidad": 2
    }
  ]
}
```

Una creación exitosa devuelve 201, la orden en estado Pendiente
y la cabecera Location con su URL de consulta.

El cliente no determina el precio: Orders lo consulta en Products.API.

## Cambios de estado

Enviar al endpoint de estado un cuerpo como:

```json
{
  "estado": "Confirmada"
}
```

| Estado actual | Destinos permitidos |
|---|---|
| Pendiente | Confirmada, Cancelada |
| Confirmada | Enviada, Cancelada |
| Enviada | Entregada |
| Entregada | Ninguno |
| Cancelada | Ninguno |

## Errores

| Código | Significado |
|---|---|
| ORD-001 | Orden o ruta no encontrada |
| ORD-002 | Solicitud inválida o incompatible |
| ORD-003 | Usuario no encontrado |
| ORD-004 | Producto no encontrado |
| ORD-005 | Stock insuficiente |
| ORD-006 | Conflicto al cambiar el estado |
| ORD-007 | Error interno |

Los errores utilizan ProblemDetails e incluyen errorCode,
errorMessage y correlationId.

Se puede enviar X-Correlation-Id para identificar una solicitud.
El identificador se devuelve en la respuesta y permite relacionarla con los logs.

## Pruebas

- [Solicitudes de ejemplo](Orders.API.http)
- [Resultados y pasos de pruebas manuales](PRUEBAS.md)

Reemplazar las variables del archivo HTTP por IDs reales antes de ejecutar
los casos válidos. Cada creación exitosa guarda una nueva orden.

## Compilación

Desde la raíz del repositorio:

```powershell
dotnet build src/Orders.API/Orders.API.csproj
```

En Windows, detener la instancia de Orders antes de recompilar si el
ejecutable está bloqueado.