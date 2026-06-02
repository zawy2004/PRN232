# Code Explanation — ProductManagementASPNETCoreMVC

This document explains how the code in this repository works at runtime, following a request through the layers and describing key implementation details.

## 1. High-level architecture
- Projects:
  - `BusinessObjects`: POCO entity classes (domain models).
  - `DataAccessObjects`: `MyStoreContext` (EF Core DbContext), DAOs that use `MyStoreContext` for queries.
  - `Repositories`: repository adapters that call DAOs.
  - `Services`: business/service layer used by controllers.
  - `ProductWebAPI`: ASP.NET Core Web API (startup, controllers, configuration).

## 2. Startup and configuration (`ProductWebAPI/Program.cs`)
- The app creates a WebApplication builder and configures JSON serialization options:
  - `ReferenceHandler.IgnoreCycles` to avoid serializer exceptions with navigation properties.
- Registers implementations in the DI container:
  - `IProductRepository` -> `ProductRepository` (Singleton)
  - `IProductService` -> `ProductService` (Singleton)
- Adds controllers and Swagger, builds and runs the app.

Note: the project does not register `MyStoreContext` with DI. DAOs create contexts manually.

## 3. Request handling flow (example: GET /api/Products)
1. HTTP request -> ASP.NET routing -> `ProductsController.GetProducts()`.
2. Controller is constructed by DI; it receives `IProductService` as a constructor parameter.
3. Controller calls `GetProducts()` on the service.
4. `ProductService.GetProducts()` delegates to `IProductRepository.GetProducts()`.
5. `ProductRepository.GetProducts()` delegates to `ProductDAO.GetProducts()`.
6. `ProductDAO.GetProducts()` executes EF Core code using a `MyStoreContext` instance:
   - Opens a context with `using var db = new MyStoreContext();`
   - Runs `db.Products.Include(p => p.Category).ToList()` to eager-load categories.
   - Returns the list back through repository -> service -> controller.
7. Controller returns `ActionResult<IEnumerable<Product>>`. ASP.NET serializes the EF entities to JSON, using the configured `JsonSerializerOptions`.

## 4. `MyStoreContext` behavior
- `MyStoreContext` inherits from `DbContext` and defines `DbSet<AccountMember>`, `DbSet<Category>`, `DbSet<Product>`.
- OnConfiguring:
  - If options aren't configured, it builds an `IConfiguration` reading `appsettings.json` from the current working directory and calls `optionsBuilder.UseSqlServer(GetConnectionString())`.
  - This means the DbContext obtains the connection string at runtime; it expects `appsettings.json` to be accessible from the process working directory (the startup project folder when running `dotnet run --project ProductWebAPI`).
- OnModelCreating:
  - Configures keys, column names, property lengths, types, relationships.
  - Defines `HasData(...)` seed data for `AccountMember`, `Category`, and `Product`.

## 5. DAOs, Repositories, Services
- DAOs (in `DataAccessObjects`) are static helper classes that instantiate `MyStoreContext` directly and perform EF operations. Example methods:
  - `GetProducts()`: returns a list with eager-loaded category.
  - `SaveProduct(Product p)`: `context.Products.Add(p); context.SaveChanges();`
  - `UpdateProduct(...)`, `DeleteProduct(...)`, `GetProductById(...)`.
- `ProductRepository` is a thin adapter that calls the DAO static methods.
- `ProductService` provides the service API used by controllers. In this codebase `ProductService` constructs a `ProductRepository` using `new ProductRepository()` (it does not accept the repository via constructor injection), although `Program.cs` registers `IProductService` and `IProductRepository` in DI.

Implication: behavior works but mixing `new` and DI reduces testability and defeats the point of registering implementations for constructor injection.

## 6. Entities (BusinessObjects)
- `AccountMember`:
  - `MemberId` (string, key, max length 20), `MemberPassword`, `FullName`, `EmailAddress`, `MemberRole`.
- `Category`:
  - `CategoryId` (int, key), `CategoryName` (string, max length 15), navigation `ICollection<Product> Products`.
- `Product`:
  - `ProductId` (int, key), `ProductName` (string, max length 40), `CategoryId` (FK), `UnitsInStock` (short?), `UnitPrice` (decimal? mapped to SQL `money`), navigation `Category`.

DataAnnotations + `OnModelCreating` ensure EF maps to the expected schema and column names.

## 7. Migrations & Seeding
- Migrations are generated in `DataAccessObjects/Migrations` by running `dotnet ef migrations add <Name> --project DataAccessObjects --startup-project ProductWebAPI`.
- Applying migrations is done with `dotnet ef database update --project DataAccessObjects --startup-project ProductWebAPI`.
- Seed data defined by `HasData` is translated into insert statements in the generated migration. When the migration runs, those rows are inserted into the DB (unless already present).

## 8. JSON serialization and navigation properties
- Controllers return EF entities with navigation properties. To avoid reference loop issues, `Program.cs` configures `ReferenceHandler.IgnoreCycles`.
- For stable API contracts and security, prefer projecting EF results into DTOs (Data Transfer Objects) in the service layer. DTOs let you control fields, avoid lazy-loading surprises, and prevent accidental model binding updates.

## 9. Error handling and logging
- Some DAO methods swallow exceptions (e.g., `GetProducts()` returns empty list on error) — this hides problems.
- Other methods wrap and rethrow `new Exception(e.Message)`, which loses the original stack trace and exception type.
- Recommendation: log the exception (use `ILogger<T>`) and either rethrow the original exception or throw a custom exception with the original as inner exception.

## 10. Running and testing
- Build and run:

```bash
dotnet restore
dotnet build
dotnet run --project ProductWebAPI
```

- API base URL is printed to console (e.g., `http://localhost:5230`). Test endpoints:
  - `GET /api/Products`
  - `GET /api/Products/{id}`

- Quick PowerShell test:

```powershell
Invoke-RestMethod -Uri "http://localhost:5230/api/Products"
```

## 11. Common changes you may want to make
- Register `MyStoreContext` with DI:

```csharp
builder.Services.AddDbContext<MyStoreContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MyStockDB")));
```

Then change DAOs/services to accept `MyStoreContext` (or repositories) via constructor injection.

- Create DTOs and map with LINQ projections or AutoMapper.
- Improve exception handling and add `ILogger<>` to classes for structured logging.

## 12. Files map (quick)
- `BusinessObjects/` — entity classes
- `DataAccessObjects/MyStoreContext.cs` — DbContext and seed
- `DataAccessObjects/ProductDAO.cs` — EF queries for products
- `Repositories/ProductRepository.cs` — adapter calling DAO
- `Services/ProductService.cs` — business API (used by controller)
- `ProductWebAPI/Controllers/ProductsController.cs` — HTTP endpoints
- `ProductWebAPI/Program.cs` — startup

---

File created: `CODE_EXPLANATION.md`.
