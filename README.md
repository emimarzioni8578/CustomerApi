# CustomersApi

Ejemplo didáctico de un **CRUD de clientes** con **.NET 10**, **Entity Framework Core 10** (SQL Server) y **Swagger UI**.
Las capas están separadas en **carpetas** dentro de un único proyecto.

Para dar la clase: [docs/Guia-de-clase.md](docs/Guia-de-clase.md) (también en [PDF](docs/Guia-de-clase.pdf)).

## Estructura

```
CustomersApi/
├── global.json                 # Fija el SDK de .NET 10
├── dotnet-tools.json           # Herramienta local dotnet-ef (versión 10)
└── CustomersApi/
    ├── Controllers/            # HTTP: recibe requests y devuelve códigos de estado
    ├── Models/                 # DTOs: CustomerRequest (entrada) y CustomerResponse (salida)
    ├── Mappers/                # Conversión DTO <-> Entidad
    ├── Services/               # Reglas de negocio (ej.: email único)
    │   └── Exceptions/
    ├── DataAccess/
    │   ├── CustomersDbContext.cs
    │   ├── Configurations/     # Fluent API: largos, índices, datos semilla
    │   └── Repositories/       # Única capa que usa EF Core
    ├── Entities/               # Customer: lo que se guarda en la base
    ├── Migrations/             # Generadas por EF Core (no se editan a mano)
    └── Program.cs              # Inyección de dependencias + Swagger
```

Recorrido de una petición:

```
HTTP -> CustomersController -> CustomersService -> CustomersRepository -> CustomersDbContext -> SQL Server
          (DTO <-> Entidad)      (reglas)             (EF Core)
```

## Cómo ejecutarlo

1. Revisa la cadena de conexión en `CustomersApi/appsettings.json` (usa `.\SQLEXPRESS01`, igual que MiPrimerApi).
   Para usar LocalDB: `Server=(localdb)\\MSSQLLocalDB;Database=CustomersApiDb;Trusted_Connection=True;TrustServerCertificate=True;`
2. Desde la carpeta raíz `CustomersApi/`:

```powershell
dotnet tool restore                                   # instala dotnet-ef 10 (local)
dotnet ef database update --project CustomersApi      # crea la base y aplica las migraciones
dotnet run --project CustomersApi
```

3. Abre http://localhost:5079/swagger (o usa `CustomersApi.http` desde Visual Studio o VS Code).

## Endpoints

| Método | Ruta                  | Respuestas          |
|--------|-----------------------|---------------------|
| GET    | `/api/customers`      | 200                 |
| GET    | `/api/customers/{id}` | 200, 404            |
| POST   | `/api/customers`      | 201, 400, 409       |
| PUT    | `/api/customers/{id}` | 204, 400, 404, 409  |
| DELETE | `/api/customers/{id}` | 204, 404            |

- **400**: falla una validación de `CustomerRequest` (`[Required]`, `[EmailAddress]`, ...).
- **404**: el cliente no existe.
- **409**: ya existe otro cliente con ese email.

## Migraciones con EF Core

Una **migración** es una "foto" de los cambios del modelo (entidades + configuraciones) traducida a código C#,
que EF Core convierte en SQL. Cada vez que cambias una entidad o su `Configuration`, generas una migración nueva.

El proyecto ya trae dos migraciones, que muestran cómo evoluciona la tabla:

| Migración          | Qué hace                                                                       |
|--------------------|--------------------------------------------------------------------------------|
| `InitialCreate`    | Crea la tabla `Customers`, el índice único por `Email` y 3 clientes semilla     |
| `AddCustomerPhone` | Agrega la columna `Phone` (nullable)                                           |

### Comandos más usados

Ejecútalos desde la carpeta `CustomersApi/CustomersApi` (o agrega `--project CustomersApi` desde la raíz).
Entre paréntesis está el equivalente en la *Package Manager Console* de Visual Studio.

| Para...                                       | CLI                                                  | Visual Studio (PMC)                   |
|-----------------------------------------------|------------------------------------------------------|---------------------------------------|
| Crear una migración                           | `dotnet ef migrations add NombreMigracion`           | `Add-Migration NombreMigracion`       |
| Aplicar las migraciones pendientes            | `dotnet ef database update`                          | `Update-Database`                     |
| Ver las migraciones y cuáles están aplicadas  | `dotnet ef migrations list`                          | `Get-Migration`                       |
| Volver a una migración anterior               | `dotnet ef database update InitialCreate`            | `Update-Database InitialCreate`       |
| Borrar la última migración (si NO se aplicó)  | `dotnet ef migrations remove`                        | `Remove-Migration`                    |
| Generar el script SQL (para producción)       | `dotnet ef migrations script --idempotent -o db.sql` | `Script-Migration -Idempotent`        |
| Borrar la base de datos                       | `dotnet ef database drop`                            | `Drop-Database`                       |

### Ejercicio: agregar una propiedad nueva

Así se agregó `Phone`; repítelo, por ejemplo, con `BirthDate`:

1. Agrega la propiedad a la entidad `Entities/Customer.cs`: `public DateOnly? BirthDate { get; set; }`
2. (Opcional) Configúrala en `DataAccess/Configurations/CustomerConfiguration.cs`.
3. Genera la migración: `dotnet ef migrations add AddCustomerBirthDate`
4. **Revisa** el archivo generado en `Migrations/` (métodos `Up` y `Down`).
5. Aplícala: `dotnet ef database update`
6. Agrega la propiedad a `CustomerRequest`, `CustomerResponse`, `CustomerMapper` y al `UpdateAsync` del servicio.

### Cosas a tener en cuenta

- **No edites la base a mano**: el modelo de C# es la fuente de verdad, y la base se actualiza solo con migraciones.
- **No borres ni modifiques migraciones ya aplicadas** (y menos si ya las compartiste). Si te equivocaste, crea una migración nueva.
- `Migrations/CustomersDbContextModelSnapshot.cs` es el estado actual del modelo; EF lo compara con tu código para saber qué cambió.
- El aviso *"An operation was scaffolded that may result in the loss of data"* aparece cuando el `Up` o el `Down` eliminan o achican una columna. Léelo y revisa la migración antes de aplicarla.
- Los datos de `HasData` (seed) se insertan con las migraciones. Si los cambias, EF genera `InsertData`/`UpdateData`/`DeleteData` en la siguiente migración.
- La tabla `__EFMigrationsHistory` en la base registra qué migraciones ya se aplicaron.
