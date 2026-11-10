# Consulta de ordenes antes de borrar productos

Products.API usa GET `/api/orders`, que ya aparece en la consigna. No se agrega
ningun endpoint obligatorio a Orders ni se accede a su base de datos.

La direccion se configura en `Services:Orders` o `Services__Orders`. El valor
local es `http://localhost:5165/`, el puerto HTTP de Orders.API.

Products necesita que el listado devuelva el JSON de la consigna, con estado e
items.productoId. Los demas campos pueden estar presentes: se ignoran en esta
consulta. Ejemplo reducido:

```json
[
  {
    "estado": "Pendiente",
    "items": [
      { "productoId": "3fa85f64-5717-4562-b3fc-2c963f66afa6", "cantidad": 2, "precioUnitario": 1500 }
    ]
  }
]
```

Si el producto figura en una orden Pendiente o Confirmada, se responde 409
PRD-004 y no se elimina. Enviada, Entregada y Cancelada no bloquean el borrado.
Una lista vacia permite eliminar. Si el producto no existe, se devuelve PRD-001
sin consultar Orders.

Si Orders no responde, devuelve un error o entrega datos invalidos, Products
devuelve 500 PRD-005 y conserva el producto. No se interpreta una falla como
ausencia de ordenes. El timeout es de cinco segundos y se propaga el encabezado
X-Correlation-Id.

Por esto, para probar un DELETE exitoso hay que tener Orders real disponible.
Iniciar Users en 5290, Products en 5208 y Orders en 5165 con los perfiles http.
Crear un usuario y un producto; usar sus IDs para crear una orden. Con la orden
Pendiente, el DELETE del producto debe devolver 409 PRD-004. Tras pasarla a
Cancelada, repetir el DELETE: debe devolver 204. Las solicitudes de ejemplo
estan en src/Orders.API/Orders.API.http.

La consulta valida el estado recibido en ese momento; no reserva stock ni
agrega transacciones distribuidas.
