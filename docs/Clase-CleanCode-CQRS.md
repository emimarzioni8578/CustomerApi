# Clase: Clean Code y CQRS sobre una API de catálogo

**Duración:** 2 horas · **Proyecto de práctica:** `CatalogClean/` · **Proyecto de comparación:** `ProductCatalogApi/`

## Objetivos

Al terminar la clase, cada participante puede:

1. Reconocer en código real los principios de Clean Code que más impacto tienen (nombres, funciones chicas, una responsabilidad, errores explícitos).
2. Explicar la regla de dependencia de Clean Architecture y ubicar una clase en la capa correcta.
3. Distinguir un command de una query y justificar por qué se separan.
4. Agregar un caso de uso completo (command, handler, endpoint y test) al proyecto.

## Antes de empezar

- Ejecutar `docs/CatalogoDb.sql` en `.\SQLEXPRESS01` (crea `CatalogoDb` con datos iniciales).
- Establecer `Catalog.Api` como proyecto de inicio. Swagger UI abre en `http://localhost:5310/swagger`.
- Tener a mano `CatalogClean/Catalog.Api/Catalog.Api.http` para lanzar peticiones.

## Agenda

| Tiempo | Bloque | Tipo |
|---|---|---|
| 0:00 – 0:10 | 1. Punto de partida: `ProductCatalogApi` | Discusión |
| 0:10 – 0:35 | 2. Clean Code | Teoría con ejemplos del proyecto |
| 0:35 – 0:50 | 3. Clean Architecture | Teoría |
| 0:50 – 0:55 | Pausa | |
| 0:55 – 1:15 | 4. CQRS | Teoría |
| 1:15 – 1:45 | 5. Práctica guiada | Código en vivo |
| 1:45 – 1:55 | 6. Ejercicios | Individual o en parejas |
| 1:55 – 2:00 | 7. Cierre | Preguntas de repaso |

---

## 1. Punto de partida (10 min)

Abrir `ProductCatalogApi`. Ya está en capas (controller → `ProductService` → repositorio) y ya usa el patrón Result. Para un CRUD chico es una opción válida. Mirar `ProductsController.PutProduct` y preguntar al grupo qué sigue faltando:

```csharp
[HttpPut("{id:int}")]
public async Task<IActionResult> PutProduct(int id, Producto producto)
{
	if (id != producto.ProductoId)
	{
		return BadRequest();
	}

	await _productService.UpdateProductAsync(id, producto);

	return NoContent();
}
```

Puntos a hacer notar:

- **El `Result` del servicio se ignora.** `PutProduct` y `DeleteProduct` responden 204 aunque el producto no exista. El patrón solo sirve si el controller lo revisa siempre.
- **La entidad es el contrato de la API en el PUT.** Recibe `Producto` completo: la forma de la tabla queda expuesta y el cliente puede enviar campos que la API ignora (`FechaAlta`, `Activo`).
- **La entidad no protege su estado.** `Producto` tiene setters públicos; las reglas viven aparte, en un validador que alguien tiene que acordarse de llamar.
- **Las capas son carpetas, no fronteras.** Todo está en un solo proyecto: nada impide que un controller use el `DbContext` directamente.
- **`PutProduct` no dice qué intención tiene el usuario.** ¿Cambió el precio? ¿Ajustó stock? ¿Lo dio de baja?

El resto de la clase resuelve estos cinco puntos.

---

## 2. Clean Code (25 min)

> Código limpio es código que otra persona puede leer, entender y cambiar sin miedo.

### 2.1 Nombres que revelan intención

| En lugar de | Usar | Por qué |
|---|---|---|
| `PutProduct` | `UpdateProductCommand`, `AdjustProductStockCommand` | El nombre describe la intención de negocio, no el verbo HTTP |
| `Get(int? c, bool a)` | `ListAsync(int? categoryId, bool onlyActive)` | No hace falta abrir el método para saber qué filtra |
| `Manager`, `Helper`, `Data` | `ProductReadService`, `ValidationBehavior` | El nombre dice qué hace la clase |

Regla práctica: si necesitás un comentario para explicar un nombre, cambiá el nombre.

### 2.2 Funciones chicas que hacen una sola cosa

`CreateProductCommandHandler.Handle` se lee de arriba hacia abajo como una receta: verificar categoría, verificar código, crear, guardar. Cada paso está en un solo nivel de abstracción; el *cómo* se valida el formato del código está en `Product.Create`, no en el handler.

### 2.3 Una sola razón para cambiar (SRP)

| Clase | Cambia cuando... |
|---|---|
| `ProductsController` | cambia el contrato HTTP |
| `CreateProductCommandValidator` | cambian las reglas de forma de la entrada |
| `CreateProductCommandHandler` | cambia el flujo del caso de uso |
| `Product` | cambia una regla del negocio |
| `ProductConfiguration` | cambia el esquema de la base |

Comparar con `ProductService` del bloque 1, que cambia cada vez que cambia cualquier caso de uso de productos.

### 2.4 Objetos que protegen su estado

En `Catalog.Domain/Entities/Product.cs` los setters son privados y el constructor también. La única forma de obtener un `Product` es `Product.Create(...)`, que rechaza datos inválidos. No puede existir en memoria un producto con precio cero.

```csharp
public void AdjustStock(int quantity)
{
	if (!CanAdjustStock(quantity))
	{
		throw new DomainException($"Stock insuficiente: hay {Stock} unidades y se intentó descontar {-quantity}.");
	}

	Stock += quantity;
}
```

Contrastar con `Producto` (scaffold), donde cualquier código puede escribir `producto.Stock = -5`.

### 2.5 Errores explícitos

- **Patrón Result** para los fallos esperados (no existe, duplicado, entrada inválida): el handler devuelve `Result` o `Result<T>` con un `Error` tipado (`Catalog.Application/Results`). Es el mismo patrón de `ProductCatalogApi/Business/Results`. La firma del método ya avisa que puede fallar.
- Cada error tiene nombre y vive en un solo lugar: `ProductErrors.NotFound(id)`, `CategoryErrors.HasProducts(name)`.
- **Excepciones** solo para lo que no debería pasar: `DomainException` cuando se rompe un invariante.
- Un `Error` se traduce a HTTP en **un solo lugar**: `Catalog.Api/Mappers/ResultMapper.cs` (`ToProblem`). Los controllers no tienen `try/catch`: preguntan `result.IsFailure`.

### 2.6 Sin números ni textos mágicos

`Product.NameMaxLength` y `Product.CodePattern` se definen una vez y los usan el dominio, el validador y la configuración de EF.

### 2.7 Comentarios

El código explica el *qué*; un comentario solo se justifica para el *por qué*. Ejemplo válido: `Fakes/GeneratedId.cs` explica por qué usa reflexión.

### 2.8 SOLID en una línea cada uno

- **S**: una razón para cambiar (2.3).
- **O**: agregar un caso de uso es agregar un archivo, no modificar los existentes.
- **L**: `InMemoryProductRepository` reemplaza a `ProductRepository` sin que el handler lo note.
- **I**: `IProductReadService` y `IProductRepository` son interfaces chicas y separadas.
- **D**: el handler depende de `IProductRepository`, no de EF Core.

### 2.9 Tests legibles

Abrir `Catalog.UnitTests/Products/CreateProductCommandHandlerTests.cs`. Nombre con formato `Método_Escenario_Resultado` y cuerpo en tres bloques: preparar, ejecutar, verificar.

---

## 3. Clean Architecture (15 min)

### La regla de dependencia

Las dependencias de código apuntan siempre hacia adentro. El dominio no conoce a nadie.

```
Catalog.Api  ──────────────►  Catalog.Application  ──►  Catalog.Domain
	 │                                 ▲
	 └──►  Catalog.Infrastructure  ────┘
```

| Proyecto | Contiene | Referencia a |
|---|---|---|
| `Catalog.Domain` | Entidades, reglas, interfaces de repositorio | Nada (ni un paquete NuGet) |
| `Catalog.Application` | Casos de uso: commands, queries, validadores, DTOs | Domain |
| `Catalog.Infrastructure` | EF Core, mapeo a tablas, repositorios | Application, Domain |
| `Catalog.Api` | Controllers, manejo de errores, arranque | Application, Infrastructure |

### Inversión de dependencias en acción

`CreateProductCommandHandler` necesita guardar un producto, pero Application no puede referenciar a Infrastructure. Solución: la interfaz `IProductRepository` vive adentro (Domain) y la implementación `ProductRepository` afuera (Infrastructure). `Catalog.Api/Program.cs` las conecta:

```csharp
builder.Services.AddApplication();
builder.Services.AddInfrastructure(connectionString);
```

Api referencia a Infrastructure solo para este registro; ningún controller usa sus clases (son `internal`).

### El dominio no se parece a la base

La base está en español y es heredada (`Productos.ProductoId`, `Precio`, `Activo`). El dominio usa `Product.Id`, `Price`, `IsActive`. La traducción está aislada en `Infrastructure/Persistence/Configurations/ProductConfiguration.cs`. Si el DBA renombra una columna, se toca un archivo.

### Costo

Más proyectos y más archivos. Para un CRUD de tres tablas que no va a crecer, `ProductCatalogApi` es suficiente. Esta estructura se paga sola cuando hay reglas de negocio, varios desarrolladores o necesidad de tests.

---

## 4. CQRS (20 min)

### De CQS a CQRS

**CQS** (Bertrand Meyer): un método o cambia estado o devuelve datos, nunca las dos cosas.
**CQRS** (Command Query Responsibility Segregation) lleva la idea a nivel de modelo: un modelo para escribir y otro para leer.

| | Command | Query |
|---|---|---|
| Qué hace | Cambia estado | Devuelve datos |
| Nombre | Verbo imperativo: `CreateProduct`, `AdjustProductStock` | `GetProducts`, `GetProductById` |
| Devuelve | `Result`, o `Result<int>` con el id creado | Un DTO, o `Result<Dto>` si puede no existir |
| Pasa por | Entidad de dominio + repositorio + `IUnitOfWork` | Servicio de lectura con proyección directa |
| Efectos secundarios | Sí | Ninguno |

### Por qué separar

- **Escribir y leer necesitan cosas distintas.** Escribir necesita reglas e invariantes. Leer necesita velocidad y la forma exacta que pide la pantalla.
- **Las lecturas no pagan el costo del dominio.** `ProductReadService` usa `AsNoTracking`, hace el `JOIN` con categorías y proyecta directo a `ProductDto`. No carga entidades ni pasa por el repositorio.
- **Commands con intención.** `AdjustProductStockCommand(Id, Quantity)` dice más que un `PUT` con el producto entero, y solo puede tocar el stock.
- **Cada caso de uso es una clase chica**, fácil de probar y de encontrar.

### Lo que CQRS no es

- No exige dos bases de datos. Acá hay una sola (`CatalogoDb`).
- No es Event Sourcing. Suelen aparecer juntos, pero son independientes.
- No exige MediatR. Es un patrón; MediatR solo resuelve el despacho.

### MediatR en el proyecto

1. El controller envía un mensaje: `sender.Send(new GetProductByIdQuery(id))`.
2. MediatR busca el único handler de ese mensaje.
que corre los validadores de FluentValidation y, si algo falla, devuelve un `Result` fallido con los errores por campo sin llegar al handler.

```
HTTP → Controller → ISender.Send → ValidationBehavior → Handler → Repositorio / Servicio de lectura → SQL Server
```

hacen visible en la firma de qué lado está cada mensaje. Además, todo `ICommand` devuelve un `Result`.

Se usa **MediatR 12.5.0** a propósito: es la última versión con licencia Apache 2.0. Desde la 13 requiere licencia comercial.

### Validación en dos niveles

| Nivel | Dónde | Pregunta que responde | Resultado |
|---|---|---|---|
| Entrada | `CreateProductCommandValidator` | ¿La petición está bien formada? | 400 con todos los errores por campo |
| Invariante | `Product.Create` | ¿Este objeto puede existir? | `DomainException`, primer error |
| Estado | Handler | ¿Es válido contra lo que ya hay? (no existe, código duplicado, stock insuficiente) | `Result` fallido: 404 o 409 |

Hay algo de repetición entre los dos primeros. Es intencional: el dominio no puede confiar en que siempre lo llamen a través del pipeline.

### Cuándo no usarlo

CRUD puro sin reglas, prototipos, equipos muy chicos. CQRS agrega archivos e indirección; si no hay diferencia real entre leer y escribir, no aporta.

### Para discutir

La unicidad del código se verifica en el handler con `ExistsByCodeAsync`. Si dos peticiones llegan a la vez, ambas pasan la verificación y la segunda falla en la restricción `UQ_Productos_Codigo`, con un 500. ¿Dónde y cómo lo manejarían sin que Application conozca EF Core?

---

## 5. Práctica guiada (30 min)

### 5.1 Recorrer una query (5 min)

Ejecutar `GET /api/products/1` y seguir el camino con el depurador:

1. `ProductsController.GetProduct`
2. `GetProductByIdQueryHandler.Handle`
3. `ProductReadService.GetByIdAsync`. Mirar el SQL en la ventana de salida: un solo `SELECT` con `INNER JOIN`.

y seguir el `Result` fallido: el handler devuelve `ProductErrors.NotFound`, el controller ve `IsFailure` y `ToProblem` lo convierte en un 404.

### 5.2 Recorrer un command (10 min)

Enviar el `POST` inválido del archivo `.http` (código `tec4`, precio `0`). Poner un punto de interrupción en `ValidationBehavior` y otro en el handler: el handler no se ejecuta. La respuesta es un 400 con un error por campo.

Enviar el `POST` válido y seguir:

1. `ValidationBehavior` (pasa).
2. `CreateProductCommandHandler`: categoría existe, código libre.
3. `Product.Create`: invariantes.
4. `SaveChangesAsync`: mirar el `INSERT`. `FechaAlta` no se envía; la completa el valor por defecto de la base.

Enviar `DELETE /api/categories/1`: 409, porque tiene productos.

### 5.3 Agregar un caso de uso en vivo: dar de baja un producto (15 min)

El dominio ya tiene `Product.Deactivate()`. Falta exponerlo.

**Paso 1.** Crear `Catalog.Application/Products/Commands/DeactivateProduct.cs`:

```csharp
using Catalog.Application.Abstractions.Messaging;
using Catalog.Application.Results;
using Catalog.Domain.Repositories;
using MediatR;

namespace Catalog.Application.Products.Commands;

public sealed record DeactivateProductCommand(int Id) : ICommand;

public sealed class DeactivateProductCommandHandler(IProductRepository products, IUnitOfWork unitOfWork)
	: IRequestHandler<DeactivateProductCommand, Result>
{
	public async Task<Result> Handle(DeactivateProductCommand command, CancellationToken cancellationToken)
	{
		var product = await products.GetByIdAsync(command.Id, cancellationToken);

		if (product is null)
		{
			return Result.Failure(ProductErrors.NotFound(command.Id));
		}

		product.Deactivate();

		await unitOfWork.SaveChangesAsync(cancellationToken);

		return Result.Success();
	}
}
```

**Paso 2.** Agregar el endpoint en `ProductsController`:

```csharp
[HttpPost("{id:int}/deactivate")]
public async Task<IActionResult> DeactivateProduct(int id, CancellationToken cancellationToken)
{
	var result = await sender.Send(new DeactivateProductCommand(id), cancellationToken);

	if (result.IsFailure)
	{
		return result.Error!.ToProblem();
	}

	return NoContent();
}
```

id inexistente devuelve un error `NotFound`. Usar `AdjustProductStockCommandHandlerTests` como modelo.

**Paso 4.** Probar: `POST /api/products/1/deactivate` y luego `GET /api/products?onlyActive=true`.

Hacer notar qué **no** hubo que tocar: `Program.cs`, el `DbContext`, los repositorios ni ningún otro handler. MediatR descubre el handler nuevo solo.

---

## 6. Ejercicios (10 min en clase; el resto queda de tarea)

1. **Reactivar un producto.** `ActivateProductCommand`, handler, endpoint `POST /api/products/{id}/activate` y test.
`GetProductByCodeQuery` que devuelva `Result<ProductDto>` (404 si no existe).
3. **Tests de la actualización.** Para `UpdateProductCommandHandler`: actualización válida, producto inexistente y categoría inexistente (los dos últimos devuelven `NotFound` y no llaman a `SaveChanges`).
4. **Un segundo behavior.** `LoggingBehavior<TRequest, TResponse>` que registre el nombre del mensaje y el tiempo que tardó. Registrarlo en `Application/DependencyInjection.cs`. ¿Importa el orden respecto de `ValidationBehavior`?
5. **Refactor (Clean Code).** En `UpdateCategoryCommandHandler` y `CreateCategoryCommandHandler` se repite `command.Name.Trim()`. Proponer una forma de eliminar la repetición y justificarla.
6. **Para pensar.** Resolver la pregunta de la carrera sobre el código duplicado del bloque 4.

---

## 7. Cierre (5 min)

Preguntas de repaso:

1. ¿Qué proyecto no puede tener ninguna referencia a otro? ¿Por qué?
2. ¿Por qué `GetProductsQueryHandler` no usa `IProductRepository`?
3. Un `POST` con precio `0`: ¿qué clase lo rechaza primero y qué código HTTP devuelve?
4. ¿Qué diferencia hay entre `UpdateProductCommand` y `AdjustProductStockCommand` desde el punto de vista del negocio?
5. Nombrar un caso en el que **no** usarían CQRS.

Respuestas esperadas, en breve: (1) `Catalog.Domain`; es lo más estable y todo depende de él. (2) Una lectura no necesita entidades ni invariantes; proyecta directo al DTO. (3) `CreateProductCommandValidator` a través de `ValidationBehavior`; 400. (4) El primero edita datos descriptivos; el segundo expresa una operación concreta y solo puede tocar el stock. (5) CRUD sin reglas, prototipos.

## Mapa del proyecto

```
CatalogClean/
├── Catalog.Domain/
│   ├── Entities/            Product, Category
│   ├── Exceptions/          DomainException
│   └── Repositories/        IProductRepository, ICategoryRepository, IUnitOfWork
├── Catalog.Application/
│   ├── Abstractions/        ICommand, IQuery
│   ├── Behaviors/           ValidationBehavior
│   ├── Results/             Result, Result<T>, Error
│   ├── Products/            ProductDto, ProductErrors, IProductReadService, Commands/, Queries/
│   └── Categories/          CategoryDto, CategoryErrors, ICategoryReadService, Commands/, Queries/
├── Catalog.Infrastructure/
│   └── Persistence/         CatalogDbContext, Configurations/, Repositories/, ReadServices/
├── Catalog.Api/
│   ├── Controllers/         ProductsController, CategoriesController
│   ├── Contracts/           Cuerpos de PUT y PATCH
│   ├── Mappers/             ResultMapper (Error → ProblemDetails)
│   └── ExceptionHandling/   GlobalExceptionHandler (solo DomainException)
└── Catalog.UnitTests/       Fakes/, Entities/, Behaviors/, Products/, Categories/
```

Cada caso de uso está en un solo archivo (mensaje, validador y handler juntos): lo que cambia junto, vive junto.

## Endpoints

| Verbo | Ruta | Mensaje |
|---|---|---|
| GET | `/api/products?categoryId=&onlyActive=` | `GetProductsQuery` |
| GET | `/api/products/{id}` | `GetProductByIdQuery` |
| POST | `/api/products` | `CreateProductCommand` |
| PUT | `/api/products/{id}` | `UpdateProductCommand` |
| PATCH | `/api/products/{id}/stock` | `AdjustProductStockCommand` |
| DELETE | `/api/products/{id}` | `DeleteProductCommand` |
| GET | `/api/categories` | `GetCategoriesQuery` |
| GET | `/api/categories/{id}` | `GetCategoryByIdQuery` |
| POST | `/api/categories` | `CreateCategoryCommand` |
| PUT | `/api/categories/{id}` | `UpdateCategoryCommand` |
| DELETE | `/api/categories/{id}` | `DeleteCategoryCommand` |
