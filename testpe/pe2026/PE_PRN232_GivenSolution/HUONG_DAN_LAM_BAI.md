# Hướng dẫn làm nhanh PE PRN232 – FPTGear

> Mục tiêu: **chạy được, đủ điểm**, không cần kiến trúc đẹp. Không tách Business layer, không generic UnitOfWork, không ProjectTo. 85 phút thôi.

## 0. Việc cần biết trước

- `Q1` = Web API (đã có sẵn package AutoMapper, JwtBearer, EFCore, OData, Swagger). Làm Question 1 ở đây.
- `Q2` = MVC rỗng, có sẵn `Utilities.GetAbsoluteUrl(path)` để gọi API Q1 — **không sửa file `Utilities.cs`**. Làm Question 2 ở đây.
- `FPTGearDB.sql` là DB có sẵn dữ liệu (5 Equipments, 8 Rentals, 5 Users: `admin@fptgear.com` là admin, còn lại customer, mật khẩu đều `"123456"` dạng **plain text** — login so sánh string trực tiếp, đừng hash cho mất công).
- ⚠️ **Port lệch nhau**: `Q1/Properties/launchSettings.json` = `5230`, `Q2/appsettings.json -> GivenAPIBaseUrl` = `5100`. Sửa port Q1 thành `5100` cho khớp.

---

## 1. Setup (5 phút)

1. Restore `FPTGearDB.sql` vào SQL Server (mở bằng SSMS, Execute).
2. `Q1/appsettings.json` → điền connection string vào key `MyCnn` có sẵn:
   ```json
   "MyCnn": "Server=(localdb)\\MSSQLLocalDB;Database=FPTGearDB;Trusted_Connection=True;TrustServerCertificate=True"
   ```
3. Sửa `Q1/Properties/launchSettings.json`, đổi `applicationUrl` → `http://localhost:5100`.
4. Trong `Q1`, tạo 4 folder phẳng: `Models/`, `DTOs/`, `Repositories/`, `Controllers/` (KHÔNG cần Business/DataAccess/Mappings riêng — để `DbContext` và `MappingProfile` ngay trong `Models/`, đỡ mất công điều hướng).

---

## 2. Models + DbContext (10 phút)

Viết tay thẳng, không scaffold (mất thời gian cài tool):

```csharp
// Models/Entities.cs
public class User
{
    public int UserId { get; set; }
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string Role { get; set; } = null!; // "admin" | "customer"
    public DateTime CreatedAt { get; set; }
}

public class Equipment
{
    public int EquipmentId { get; set; }
    public string Name { get; set; } = null!;
    public string Category { get; set; } = null!;
    public decimal PricePerDay { get; set; }
    public decimal DepositFee { get; set; }
    public int StockQuantity { get; set; }
    public DateTime CreatedAt { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = null!;
}

public class Rental
{
    public int RentalId { get; set; }
    public int EquipmentId { get; set; }
    public int UserId { get; set; }
    public int Quantity { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Status { get; set; } = "Active";
    public decimal FineAmount { get; set; }
    public DateTime RentalDate { get; set; }
    public Equipment Equipment { get; set; } = null!;
}
```

```csharp
// Models/FPTGearDbContext.cs
public class FPTGearDbContext : DbContext
{
    public FPTGearDbContext(DbContextOptions<FPTGearDbContext> options) : base(options) { }
    public DbSet<User> Users => Set<User>();
    public DbSet<Equipment> Equipments => Set<Equipment>();
    public DbSet<Rental> Rentals => Set<Rental>();
    // FK convention tự nhận theo tên UserId/EquipmentId -> KHÔNG cần Fluent API gì thêm
}
```

Đăng ký trong `Program.cs`:
```csharp
builder.Services.AddDbContext<FPTGearDbContext>(o =>
    o.UseSqlServer(builder.Configuration.GetConnectionString("MyCnn")));
```

---

## 3. JWT Auth (10 phút)

`appsettings.json` thêm:
```json
"Jwt": { "Key": "PRN232_EXAM_SECRET_KEY_1234567890", "Issuer": "FPTGearApi", "Audience": "FPTGearClient" }
```

```csharp
// DTOs/AuthDtos.cs
public record LoginRequestDto(string Email, string Password);
public record LoginResponseDto(string Token, string Email, string Role, int UserId);
```

```csharp
// Controllers/AuthController.cs
[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly FPTGearDbContext _db;
    private readonly IConfiguration _config;
    public AuthController(FPTGearDbContext db, IConfiguration config) { _db = db; _config = config; }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequestDto dto)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
        if (user is null || user.Password != dto.Password) return Unauthorized("Invalid email or password");

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Role, user.Role)
        };
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"], audience: _config["Jwt:Audience"],
            claims: claims, expires: DateTime.UtcNow.AddHours(4),
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

        return Ok(new LoginResponseDto(new JwtSecurityTokenHandler().WriteToken(token), user.Email, user.Role, user.UserId));
    }
}
```

`Program.cs`:
```csharp
builder.Services.AddAuthentication(o =>
{
    o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(o => o.TokenValidationParameters = new TokenValidationParameters
{
    ValidateIssuer = true, ValidateAudience = true, ValidateLifetime = true, ValidateIssuerSigningKey = true,
    ValidIssuer = builder.Configuration["Jwt:Issuer"],
    ValidAudience = builder.Configuration["Jwt:Audience"],
    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
});
builder.Services.AddAuthorization();
// ...
app.UseAuthentication();
app.UseAuthorization();
```
Dùng `[Authorize]` và `[Authorize(Roles = "admin")]` trên controller/action.

---

## 4. Equipment CRUD + Repository + AutoMapper (15 phút)

**Repository tối giản** — chỉ 1 class thẳng cho Equipment, đủ để tính điểm "Repository Pattern", không cần generic/interface phức tạp:

```csharp
// Repositories/IEquipmentRepository.cs + EquipmentRepository.cs
public interface IEquipmentRepository
{
    IQueryable<Equipment> GetQueryable();                 // cho OData + GetAll
    Task<Equipment?> GetByIdAsync(int id);
    Task AddAsync(Equipment e);
    void Update(Equipment e);
    void Remove(Equipment e);
    Task SaveAsync();
}

public class EquipmentRepository : IEquipmentRepository
{
    private readonly FPTGearDbContext _db;
    public EquipmentRepository(FPTGearDbContext db) => _db = db;

    public IQueryable<Equipment> GetQueryable() => _db.Equipments.Include(e => e.User);
    public Task<Equipment?> GetByIdAsync(int id) => _db.Equipments.Include(e => e.User).FirstOrDefaultAsync(e => e.EquipmentId == id);
    public async Task AddAsync(Equipment e) => await _db.Equipments.AddAsync(e);
    public void Update(Equipment e) => _db.Equipments.Update(e);
    public void Remove(Equipment e) => _db.Equipments.Remove(e);
    public Task SaveAsync() => _db.SaveChangesAsync();
}
```
`Program.cs`: `builder.Services.AddScoped<IEquipmentRepository, EquipmentRepository>();`

**DTOs + AutoMapper (đủ tính điểm, 1 profile duy nhất cho cả app):**
```csharp
// DTOs/EquipmentDtos.cs
public record EquipmentDto(int EquipmentId, string Name, string Category, decimal PricePerDay,
    decimal DepositFee, int StockQuantity, string UploaderEmail);
public record CreateEquipmentDto(string Name, string Category, decimal PricePerDay, decimal DepositFee, int StockQuantity);

// Models/MappingProfile.cs
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Equipment, EquipmentDto>().ForMember(d => d.UploaderEmail, o => o.MapFrom(s => s.User.Email));
        CreateMap<CreateEquipmentDto, Equipment>();
        CreateMap<Rental, RentalDto>().ForMember(d => d.EquipmentName, o => o.MapFrom(s => s.Equipment.Name));
    }
}
```
`Program.cs`: `builder.Services.AddAutoMapper(typeof(MappingProfile));`

**Controller — GetAll dùng `ToListAsync()` rồi map list (đơn giản nhất, không đụng ProjectTo):**
```csharp
[ApiController]
[Route("api/equipments")]
public class EquipmentsController : ControllerBase
{
    private readonly IEquipmentRepository _repo;
    private readonly IMapper _mapper;
    public EquipmentsController(IEquipmentRepository repo, IMapper mapper) { _repo = repo; _mapper = mapper; }

    [HttpGet]
    public async Task<IActionResult> GetAll()
        => Ok(_mapper.Map<List<EquipmentDto>>(await _repo.GetQueryable().ToListAsync()));

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var eq = await _repo.GetByIdAsync(id);
        return eq is null ? NotFound() : Ok(_mapper.Map<EquipmentDto>(eq));
    }

    [HttpPost, Authorize(Roles = "admin")]
    public async Task<IActionResult> Create(CreateEquipmentDto dto)
    {
        var entity = _mapper.Map<Equipment>(dto);
        entity.CreatedAt = DateTime.Now;
        entity.UserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        await _repo.AddAsync(entity);
        await _repo.SaveAsync();
        return CreatedAtAction(nameof(GetById), new { id = entity.EquipmentId }, _mapper.Map<EquipmentDto>(entity));
    }

    [HttpPut("{id}"), Authorize(Roles = "admin")]
    public async Task<IActionResult> Update(int id, CreateEquipmentDto dto)
    {
        var entity = await _repo.GetByIdAsync(id);
        if (entity is null) return NotFound();
        _mapper.Map(dto, entity);
        _repo.Update(entity);
        await _repo.SaveAsync();
        return NoContent();
    }

    [HttpDelete("{id}"), Authorize(Roles = "admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await _repo.GetByIdAsync(id);
        if (entity is null) return NotFound();
        _repo.Remove(entity);
        await _repo.SaveAsync();
        return NoContent();
    }

    // Riêng cho OData (mục 6) — trả entity trực tiếp, route tách biệt
    [HttpGet, Route("/odata/Equipments"), EnableQuery]
    public IQueryable<Equipment> GetOData() => _repo.GetQueryable();
}
```
GET không gắn `[Authorize]` → public cho mọi user, đúng đề "All users".

---

## 5. Rental API (15 phút)

**Không cần Repository riêng cho Rental — dùng thẳng `FPTGearDbContext` trong controller.** Nhanh hơn, và Repository Pattern đã được thể hiện đủ ở Equipment rồi.

```csharp
// DTOs/RentalDtos.cs
public record CreateRentalDto(int EquipmentId, int Quantity, DateTime StartDate, DateTime EndDate);
public record RentalDto(int RentalId, int EquipmentId, string EquipmentName, int UserId, int Quantity,
    DateTime StartDate, DateTime EndDate, string Status, decimal FineAmount, DateTime RentalDate);
```

```csharp
[ApiController]
[Route("api/rentals")]
[Authorize]
public class RentalsController : ControllerBase
{
    private readonly FPTGearDbContext _db;
    private readonly IMapper _mapper;
    public RentalsController(FPTGearDbContext db, IMapper mapper) { _db = db; _mapper = mapper; }

    [HttpPost]
    public async Task<IActionResult> Create(CreateRentalDto dto)
    {
        var equipment = await _db.Equipments.FindAsync(dto.EquipmentId);
        if (equipment is null) return NotFound("Equipment not found");
        if (dto.Quantity <= 0 || dto.Quantity > equipment.StockQuantity) return BadRequest("Invalid quantity or not enough stock");

        equipment.StockQuantity -= dto.Quantity;
        var rental = new Rental
        {
            EquipmentId = dto.EquipmentId,
            UserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!),
            Quantity = dto.Quantity, StartDate = dto.StartDate, EndDate = dto.EndDate,
            Status = "Active", FineAmount = 0, RentalDate = DateTime.Now
        };
        _db.Rentals.Add(rental);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = rental.RentalId }, _mapper.Map<RentalDto>(rental));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var rental = await _db.Rentals.Include(r => r.Equipment).FirstOrDefaultAsync(r => r.RentalId == id);
        return rental is null ? NotFound() : Ok(_mapper.Map<RentalDto>(rental));
    }

    [HttpPatch("{id}/return")]
    public async Task<IActionResult> Return(int id)
    {
        var rental = await _db.Rentals.Include(r => r.Equipment).FirstOrDefaultAsync(r => r.RentalId == id);
        if (rental is null) return NotFound();
        if (rental.Status == "Returned") return BadRequest("Already returned");

        if (DateTime.Now > rental.EndDate)
        {
            var daysLate = (DateTime.Now.Date - rental.EndDate.Date).Days;
            rental.FineAmount = 0.1m * rental.Equipment.PricePerDay * daysLate * rental.Quantity;
        }
        rental.Status = "Returned";
        rental.Equipment.StockQuantity += rental.Quantity;
        await _db.SaveChangesAsync();
        return Ok(_mapper.Map<RentalDto>(rental));
    }
}
```
`UserId` lấy từ JWT claim, không nhận từ body → đúng yêu cầu "Customer chỉ tạo rental của chính mình".

---

## 6. OData (5 phút)

`Program.cs`:
```csharp
builder.Services.AddControllers().AddOData(o => o
    .Select().Filter().OrderBy().Count().SetMaxTop(100)
    .AddRouteComponents("odata", GetEdmModel()));

static IEdmModel GetEdmModel()
{
    var b = new ODataConventionModelBuilder();
    b.EntitySet<Equipment>("Equipments");
    return b.GetEdmModel();
}
```
Endpoint đã viết sẵn ở mục 4 (`/odata/Equipments`). Test:
```
GET /odata/Equipments?$filter=PricePerDay gt 20
GET /odata/Equipments?$orderby=Name desc
```

---

## 7. Test Postman (5 phút) — checklist tối thiểu, chụp screenshot bỏ vào Word

1. Login admin (`admin@fptgear.com`/`123456`) → lấy token.
2. Login customer (`alice@fptgear.com`/`123456`) → lấy token.
3. `GET /api/equipments` không token → 200.
4. `POST /api/equipments` không token → 401; với token customer → 403; với token admin → 201.
5. `PUT`, `DELETE /api/equipments/{id}` với token admin → OK.
6. `POST /api/rentals` với token customer → 201, stock giảm.
7. `POST /api/rentals` với quantity > stock → 400.
8. `PATCH /api/rentals/{id}/return` (rental quá hạn) → FineAmount đúng công thức, stock được cộng lại.
9. `GET /odata/Equipments?$filter=PricePerDay gt 20` và `?$orderby=Name desc`.

---

## 8. Question 2 – MVC (15 phút)

Không cần ViewModel riêng — deserialize thẳng vào cùng record DTO của Q1 (copy record `EquipmentDto`, `RentalDto` sang `Q2/Models/`).

```csharp
// Q2/Services/ApiClient.cs
public class ApiClient
{
    private readonly HttpClient _http;
    public ApiClient(HttpClient http) => _http = http;

    public async Task<List<EquipmentDto>> GetEquipmentsAsync()
        => await _http.GetFromJsonAsync<List<EquipmentDto>>(Utilities.GetAbsoluteUrl("api/equipments")) ?? new();

    public async Task<EquipmentDto?> GetEquipmentAsync(int id)
        => await _http.GetFromJsonAsync<EquipmentDto>(Utilities.GetAbsoluteUrl($"api/equipments/{id}"));

    public async Task<HttpResponseMessage> CreateRentalAsync(string token, object dto)
    {
        var req = new HttpRequestMessage(HttpMethod.Post, Utilities.GetAbsoluteUrl("api/rentals")) { Content = JsonContent.Create(dto) };
        req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return await _http.SendAsync(req);
    }

    public async Task<(bool ok, string? token, string? role)> LoginAsync(string email, string password)
    {
        var res = await _http.PostAsJsonAsync(Utilities.GetAbsoluteUrl("api/auth/login"), new { email, password });
        if (!res.IsSuccessStatusCode) return (false, null, null);
        var data = await res.Content.ReadFromJsonAsync<LoginResponseDto>();
        return (true, data!.Token, data.Role);
    }
}
```
`Program.cs` Q2: `builder.Services.AddHttpClient<ApiClient>(); builder.Services.AddSession(); ...; app.UseSession();`

- **Login page**: form gọi `LoginAsync`, lưu `Token`/`Role`/`Email` vào `HttpContext.Session`. Không cần dựng Identity.
- **`/Equipments` (Index)**: gọi `GetEquipmentsAsync()`, hiển thị bảng Name/Category/PricePerDay/StockQuantity/UploaderEmail.
- **`/Equipments/Details/{id}`**: gọi `GetEquipmentAsync(id)`, nếu `Session["Role"] == "customer"` hiện nút "Rent Now".
- **Rent form**: nhập Quantity/StartDate/EndDate; JS đơn giản `Total Deposit = DepositFee * Quantity` tính khi `oninput`; submit gọi `CreateRentalAsync(token, dto)` với token lấy từ Session.

---

## 9. Điểm kiến trúc (2 marks) — đã đủ, không cần làm thêm gì

- Repository Pattern: có `IEquipmentRepository`/`EquipmentRepository` tách khỏi Controller → đủ điểm, không cần generic hay UnitOfWork.
- AutoMapper: có `MappingProfile` dùng `CreateMap` cho Equipment & Rental → đủ điểm, không cần ProjectTo/QueryableExtensions.
- Rental dùng thẳng DbContext trong Controller là chấp nhận được (đề chỉ nói chung chung "Repository Pattern", không bắt buộc 100% entity).

---

## 10. Thứ tự làm (85 phút)

| Phút | Việc |
|---|---|
| 0–5 | Setup DB, connection string, sửa port (mục 1) |
| 5–15 | Models + DbContext (mục 2) |
| 15–25 | JWT Auth (mục 3) |
| 25–40 | Equipment CRUD + Repository + AutoMapper (mục 4) |
| 40–55 | Rental API (mục 5) |
| 55–60 | OData (mục 6) |
| 60–75 | Q2 MVC (mục 8) |
| 75–85 | Test Postman, chụp screenshot, build lại lần cuối (mục 7) |

⚠️ Build chạy được là ưu tiên số 1 — đề nói rõ *"fail to compile or run = 0 điểm"*. Thiếu thời gian thì cắt OData trước, đừng cắt phần build được.

---

## 11. Nộp bài
- Không để tên/MSSV trong code/file.
- Bắt buộc có Word chứa screenshot Postman — thiếu thì Q1 không được chấm.
