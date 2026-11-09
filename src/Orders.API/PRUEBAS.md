# Pruebas de Orders.API

## Preparación

Desde la raíz del repositorio, iniciar cada API en una terminal distinta:

dotnet run --project src/Users.API/Users.API.csproj --launch-profile http
dotnet run --project src/Products.API/Products.API.csproj --launch-profile http
dotnet run --project src/Orders.API/Orders.API.csproj --launch-profile http

Direcciones locales:

- Users: http://localhost:5290
- Products: http://localhost:5208
- Orders: http://localhost:5165
- Swagger de Orders: http://localhost:5165/swagger

## Datos necesarios

Crear o identificar un usuario existente y un producto con precio positivo
y stock suficiente.

En Orders.API.http, reemplazar usuarioId y productoId por sus IDs reales.
Después de crear una orden, copiar el ID devuelto en ordenId.

Los IDs en cero son marcadores y no sirven para las pruebas de creación válida.
Cada POST válido guarda una orden nueva.

## Resultados verificados

Pruebas manuales realizadas el 09/11/2026 mediante solicitudes HTTP.

| Caso | Resultado observado |
|---|---|
| Listar órdenes | 200 con una lista de órdenes |
| Filtrar por usuario existente | 200 con las órdenes de ese usuario |
| Filtrar por usuario sin órdenes | 200 con lista vacía |
| Consultar una orden existente | 200 con sus datos e ítems |
| Crear una orden válida | 201, estado Pendiente y cabecera Location |
| Calcular total | Cantidad 2 × precio 12,50 = total 25 |
| Agrupar productos repetidos | Cantidades 1 y 2 resultaron en un ítem de cantidad 3 y total 37,50 |
| Confirmar una orden Pendiente | 200 y estado Confirmada |
| Cambiar de Confirmada a Pendiente | 409, ORD-006 |
| Consultar una orden inexistente | 404, ORD-001 |
| Enviar usuarioId con formato inválido | 400, ORD-002 |
| Crear con cuerpo vacío {} | 400, ORD-002 |
| Enviar cantidad cero | 400, ORD-002 |
| Enviar un estado desconocido | 400, ORD-002 |
| Crear con usuario inexistente | 404, ORD-003 |
| Crear con producto inexistente | 404, ORD-004 |
| Solicitar más unidades que el stock | 422, ORD-005 |
| Superar el stock sumando productos repetidos | 422, ORD-005 |
| Enviar text/plain al crear | 415 con respuesta de error uniforme |
| Consultar una ruta inexistente | 404 con respuesta de error uniforme |
| Ejecutar DELETE sobre /api/orders | 405 con respuesta de error uniforme |
| Crear con las APIs dependientes apagadas | 500, ORD-007, sin detalles internos |
| Enviar X-Correlation-Id | El valor se conserva en la cabecera de respuesta y en el cuerpo de los errores |

## Cómo repetir los casos

Usar las solicitudes de Orders.API.http individualmente.

Para probar el cambio de estado:
1. Crear una orden nueva.
2. Copiar su ID en ordenId.
3. Ejecutar el cambio a Confirmada.
4. Intentar volver a Pendiente y comprobar el error ORD-006.

Para probar stock insuficiente:
1. Consultar el stock actual del producto.
2. Solicitar una cantidad superior.
3. Repetir con dos ítems del mismo producto cuya suma supere el stock.

Para probar ORD-007:
1. Detener Users.API.
2. Intentar crear una orden con IDs no vacíos y cantidad válida.
3. Comprobar el error 500 con código ORD-007.
4. Volver a iniciar Users.API.

## Alcance

Estas pruebas son manuales; no constituyen una suite automatizada.

La validación de stock comprueba la cantidad solicitada contra Products.API.
Estas pruebas no verifican descuento ni reserva de stock.

Quedan fuera de esta ejecución las pruebas de concurrencia, carga
y todas las combinaciones posibles de cambios de estado.