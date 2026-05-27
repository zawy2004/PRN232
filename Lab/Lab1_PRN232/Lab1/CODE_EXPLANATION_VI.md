# Giải thích mã — ProductManagementASPNETCoreMVC

Tài liệu này giải thích cách hoạt động của mã trong repository ở thời gian chạy, theo luồng một request qua các lớp, và mô tả các chi tiết quan trọng.

## 1. Kiến trúc tổng quan
- Các project:
  - `BusinessObjects`: các lớp POCO (model/domain).
  - `DataAccessObjects`: `MyStoreContext` (EF Core DbContext), các DAO dùng `MyStoreContext` để truy vấn.
  - `Repositories`: lớp adapter repository gọi DAO.
  - `Services`: lớp dịch vụ/logic nghiệp vụ được controller sử dụng.
  - `ProductWebAPI`: ASP.NET Core Web API (khởi tạo, controller, cấu hình).

## 2. Khởi động và cấu hình (`ProductWebAPI/Program.cs`)
- Ứng dụng tạo `WebApplication` builder và cấu hình JSON serializer:
  - `ReferenceHandler.IgnoreCycles` để tránh lỗi khi serializing các navigation property vòng.
- Đăng ký các implementation vào DI container:
  - `IProductRepository` -> `ProductRepository` (Singleton)
  - `IProductService` -> `ProductService` (Singleton)
- Thêm controllers và Swagger, build và chạy ứng dụng.

Lưu ý: project hiện không đăng ký `MyStoreContext` vào DI; các DAO tự khởi tạo `MyStoreContext` trực tiếp.

## 3. Luồng xử lý request (ví dụ: GET /api/Products)
1. Request HTTP được ASP.NET routing đến `ProductsController.GetProducts()`.
2. Controller được tạo bởi DI; nó nhận một instance `IProductService` qua constructor.
3. Controller gọi `GetProducts()` trên service.
4. `ProductService.GetProducts()` gọi `IProductRepository.GetProducts()`.
5. `ProductRepository.GetProducts()` gọi `ProductDAO.GetProducts()`.
6. `ProductDAO.GetProducts()` thực hiện truy vấn EF Core bằng một `MyStoreContext` mới:
   - `using var db = new MyStoreContext();`
   - `db.Products.Include(p => p.Category).ToList()` để eager-load `Category`.
   - Trả kết quả ngược lại qua repository -> service -> controller.
7. Controller trả về `ActionResult<IEnumerable<Product>>`. ASP.NET serializes các entity sang JSON dùng `JsonSerializerOptions` đã cấu hình.

## 4. Hành vi của `MyStoreContext`
- `MyStoreContext` kế thừa `DbContext` và khai báo `DbSet<AccountMember>`, `DbSet<Category>`, `DbSet<Product>`.
- `OnConfiguring`:
  - Nếu options chưa được cấu hình, nó xây `IConfiguration` đọc `appsettings.json` từ working directory hiện tại và gọi `optionsBuilder.UseSqlServer(GetConnectionString())`.
  - Điều này nghĩa là DbContext lấy connection string vào thời điểm runtime; cần chạy ứng dụng từ thư mục chứa `appsettings.json` (thường là startup project `ProductWebAPI`).
- `OnModelCreating`:
  - Cấu hình khóa, tên cột, độ dài chuỗi, kiểu cột, quan hệ.
  - Định nghĩa `HasData(...)` để seed dữ liệu mẫu (AccountMember, Category, Product).

## 5. DAO, Repository, Service
- DAO (trong `DataAccessObjects`) là các lớp static khởi tạo `MyStoreContext` và thực hiện thao tác EF. Ví dụ:
  - `GetProducts()` trả list sản phẩm kèm `Category`.
  - `SaveProduct(Product p)` thêm và `SaveChanges()`.
  - `UpdateProduct(...)`, `DeleteProduct(...)`, `GetProductById(...)`.
- `ProductRepository` là adapter mỏng gọi các phương thức DAO.
- `ProductService` cung cấp API nghiệp vụ cho controller. Hiện `ProductService` tự tạo `ProductRepository` bằng `new ProductRepository()` thay vì chấp nhận qua constructor injection, nên dù `Program.cs` có đăng ký DI, code vẫn dùng `new` làm giảm khả năng test và tính nhất quán của DI.

## 6. Các entity (BusinessObjects)
- `AccountMember`:
  - `MemberId` (string, key, max length 20), `MemberPassword`, `FullName`, `EmailAddress`, `MemberRole`.
- `Category`:
  - `CategoryId` (int, key), `CategoryName` (string, max length 15), navigation `ICollection<Product> Products`.
- `Product`:
  - `ProductId` (int, key), `ProductName` (string, max length 40), `CategoryId` (FK), `UnitsInStock` (short?), `UnitPrice` (decimal? map tới SQL `money`), navigation `Category`.

Các DataAnnotation kết hợp với cấu hình trong `OnModelCreating` đảm bảo EF map đúng sang schema SQL.

## 7. Migrations & Seed
- Migrations được tạo trong `DataAccessObjects/Migrations` bằng lệnh:

```bash
dotnet ef migrations add <Name> --project DataAccessObjects --startup-project ProductWebAPI
```

- Áp migration lên database:

```bash
dotnet ef database update --project DataAccessObjects --startup-project ProductWebAPI
```

- `HasData` sẽ được chuyển thành các lệnh INSERT trong migration, và sẽ chèn các bản ghi khi migration chạy (nếu chưa tồn tại).

## 8. JSON serialization và navigation properties
- Controller trả về entity EF có navigation property. Để tránh vòng tham chiếu, `Program.cs` đã cấu hình `ReferenceHandler.IgnoreCycles`.
- Để API ổn định hơn, nên sử dụng DTO (Data Transfer Objects) để điều khiển chính xác cấu trúc JSON trả về.

## 9. Xử lý lỗi và logging
- Một số phương thức DAO nuốt lỗi (ví dụ `GetProducts()` trả list rỗng khi có lỗi) — điều này che khuất lỗi.
- Một số nơi bắt và ném `new Exception(e.Message)` — thao tác này làm mất stack trace gốc và thông tin về exception.
- Khuyến nghị: dùng `ILogger<T>` để log lỗi, và ném lại exception gốc hoặc gói nó vào exception mới có `innerException`.

## 10. Chạy và kiểm thử
- Build và chạy:

```bash
dotnet restore
dotnet build
dotnet run --project ProductWebAPI
```

- URL base của API sẽ được in ra console (ví dụ `http://localhost:5230`). Endpoints để test:
  - `GET /api/Products`
  - `GET /api/Products/{id}`

- Kiểm thử nhanh với PowerShell:

```powershell
Invoke-RestMethod -Uri "http://localhost:5230/api/Products"
```

## 11. Thay đổi thường gặp bạn có thể làm
- Đăng ký `MyStoreContext` vào DI:

```csharp
builder.Services.AddDbContext<MyStoreContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MyStockDB")));
```

Rồi chỉnh DAOs/services nhận `MyStoreContext` hoặc repository qua constructor injection.

- Tạo DTOs và mapping (LINQ projection hoặc AutoMapper).
- Cải thiện xử lý ngoại lệ và thêm `ILogger<>` cho logging có cấu trúc.

## 12. Bản đồ file nhanh
- `BusinessObjects/` — định nghĩa entity
- `DataAccessObjects/MyStoreContext.cs` — DbContext và seed
- `DataAccessObjects/ProductDAO.cs` — truy vấn EF cho products
- `Repositories/ProductRepository.cs` — adapter gọi DAO
- `Services/ProductService.cs` — API nghiệp vụ (controller dùng)
- `ProductWebAPI/Controllers/ProductsController.cs` — endpoints HTTP
- `ProductWebAPI/Program.cs` — khởi tạo

---

File tạo: `CODE_EXPLANATION_VI.md`.
