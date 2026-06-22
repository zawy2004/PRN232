# Giải thích Source Code — FUNewsManagementSystem

Tài liệu này giải thích **toàn bộ code** đã viết cho 2 solution (`BE` và `FE`), theo từng lớp (layer), kèm code đầy đủ của những file quan trọng và giải thích **vì sao** viết như vậy — để bạn đọc hiểu sâu, không chỉ chạy được.

> Quy ước đọc: mỗi mục có 3 phần — **Code** (nguyên văn), **Giải thích từng dòng/khối quan trọng**, **Vì sao thiết kế vậy**.

---

## 0. Bản đồ tổng thể dự án

```
Assignment2/
├── FUNewsManagement.sql                  # Schema + seed gốc của đề (không sửa)
├── FUNewsManagement_Seed_App.sql         # Script bổ sung: fix bug seed + tài khoản Admin + hash mật khẩu
│
├── BE/  (DINHGIAHUY_SE18D05_A02_BE.sln)
│   └── src/
│       ├── FUNewsManagement.DataAccess/      # Lớp truy xuất dữ liệu (DAL)
│       ├── FUNewsManagement.BusinessLogic/   # Lớp nghiệp vụ (BLL)
│       └── FUNewsManagement.Api/             # Lớp trình diễn (API + OData + xác thực header)
│
└── FE/  (DINHGIAHUY_SE18D05_A02_FE.sln)
    └── src/
        └── FUNewsManagement.Web/             # ASP.NET Core MVC, gọi BE qua HttpClient
```

**Kiến trúc tổng quan (request đi qua mấy lớp):**

```
Browser ──► FE (MVC Controller) ──HTTP/JSON──► BE Api Controller ──► BusinessLogic Service ──► DataAccess Repository ──► EF Core ──► SQL Server
           (Cookie auth của FE)   (header X-Account-*) (business rule)      (CRUD thuần)
```

Hai cơ chế đăng nhập **độc lập nhau về tầng**:
- **FE** dùng **Cookie Authentication** để biết "ai đang đăng nhập trên trình duyệt" (cho `[Authorize(Roles=...)]` của MVC).
- **BE** dùng **header định danh thuần** (`X-Account-Id`, `X-Account-Name`, `X-Account-Email`, `X-Account-Role`) để biết "request này là của ai, role gì" — FE tự đọc lại claim từ cookie của chính nó và gắn vào các header này trên mọi request gửi sang BE. **Không có JWT** trong dự án này nữa — xem cảnh báo an toàn ở mục 2.3.1 và 3.1 về lý do cách này chỉ chấp nhận được vì FE là caller duy nhất.

---

## 1. Database — sửa lỗi seed gốc và bổ sung dữ liệu

### 1.1 Vấn đề phát hiện được

File gốc `FUNewsManagement.sql` seed dữ liệu Category như sau:

```sql
INSERT INTO Category (CategoryID, CategoryName, ..., ParentCategoryID, IsActive)
VALUES (1, N'Academic news', ..., 1, 1)   -- ParentCategoryID = 1, TỰ THAM CHIẾU CHÍNH NÓ!
VALUES (2, N'Student Affairs', ..., 2, 1) -- tương tự
```

Mỗi category có `ParentCategoryID` = chính `CategoryID` của nó → khi build cây (tree) để làm menu nhiều cấp, thuật toán sẽ nghĩ "category này là con của chính nó" → không category nào được xem là **root** → cây rỗng. Đây là lỗi dữ liệu mẫu của đề, không phải lỗi code.

### 1.2 File `FUNewsManagement_Seed_App.sql` (script bổ sung, KHÔNG sửa file gốc)

```sql
USE [FUNewsManagement]
GO

UPDATE [dbo].[Category] SET [ParentCategoryID] = NULL WHERE [CategoryID] = [ParentCategoryID]
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[Category] WHERE [CategoryName] = N'Technology')
BEGIN
    INSERT INTO [dbo].[Category] ([CategoryName], [CategoryDesciption], [ParentCategoryID], [IsActive])
    VALUES (N'Technology', N'Technology-related news under Academic news.', 1, 1)

    DECLARE @TechId SMALLINT = (SELECT CategoryID FROM [dbo].[Category] WHERE [CategoryName] = N'Technology')

    INSERT INTO [dbo].[Category] ([CategoryName], [CategoryDesciption], [ParentCategoryID], [IsActive])
    VALUES
        (N'.NET', N'.NET platform news.', @TechId, 1),
        (N'Java', N'Java platform news.', @TechId, 1)
END
GO
```

- Dòng `UPDATE ... WHERE CategoryID = ParentCategoryID` → set về `NULL` cho 5 category gốc → giờ chúng là **root** thật.
- Thêm "Technology" làm con của "Academic news" (id=1), rồi ".NET"/"Java" làm con của "Technology" → tạo ra cây **3 cấp** để demo menu dropdown lồng nhau đúng yêu cầu (`tin tức > công nghệ > .NET/Java`).

Phần còn lại của script:

```sql
IF NOT EXISTS (SELECT 1 FROM [dbo].[SystemAccount] WHERE [AccountID] = 0)
BEGIN
    INSERT INTO [dbo].[SystemAccount] ([AccountID], [AccountName], [AccountEmail], [AccountRole], [AccountPassword])
    VALUES (0, N'Admin', N'admin@FUNewsManagementSystem.org', 3,
            N'100000.P7muhk+q63Mj+bPS2xtSig==.BKCEi8+BfSaDsFDC5xtEBw==')
END
GO

UPDATE [dbo].[SystemAccount]
SET [AccountPassword] = N'100000.7qM0HQZzIQ2YA9lboC4RhQ==.ZBF6FLd9Ti6Dvg3nztpYLA=='
WHERE [AccountID] BETWEEN 1 AND 5
GO
```

- **Vì sao thêm `AccountID = 0` cho Admin?** Đề bài yêu cầu: *"đã có 1 tài khoản admin mặc định, email/password lấy từ `appsettings.json`"* — nghĩa là Admin **không xác thực bằng dữ liệu trong bảng `SystemAccount`**. Nhưng bảng `NewsArticle` có cột `CreatedByID`/`UpdatedByID` tham chiếu (FK) tới `SystemAccount.AccountID`. Nếu Admin sửa/tạo tin mà không có dòng nào trong `SystemAccount` đại diện cho Admin → vi phạm khoá ngoại (foreign key) ngay khi lưu. Vậy nên ta tạo **1 dòng dự phòng `AccountID = 0`** chỉ để làm "chủ sở hữu" hợp lệ cho các tin do Admin tác động — còn việc *xác thực* (kiểm tra email+password) của Admin vẫn luôn so với `appsettings.json`, **không bao giờ** so với cột `AccountPassword` của dòng này.
- **Vì sao phải hash lại mật khẩu?** Dữ liệu seed gốc lưu mật khẩu dạng plaintext (`"@1"`) — không an toàn. Ta hash bằng PBKDF2-SHA256 (xem mục 2.3) rồi cập nhật lại.

---

## 2. BACKEND — chi tiết theo từng lớp

### 2.1 DataAccess layer — "chỉ biết nói chuyện với DB"

**Nguyên tắc:** Tầng này **không** chứa logic nghiệp vụ (không kiểm tra rule gì cả), chỉ có CRUD thuần + các câu query tái sử dụng. Đây chính là **Repository Pattern** mà đề bài yêu cầu.

#### 2.1.1 Entity (sinh ra bằng `dotnet ef dbcontext scaffold` — database-first thật)

Lệnh đã chạy:
```bash
dotnet ef dbcontext scaffold "Server=(localdb)\MSSQLLocalDB;Database=FUNewsManagement;..." \
  Microsoft.EntityFrameworkCore.SqlServer -o Entities --context FUNewsManagementContext ...
```

Lệnh này **đọc trực tiếp từ SQL Server đã tồn tại** và tự sinh ra các class C# tương ứng — đúng nghĩa "database-first" (khác với "code-first" là viết class C# trước rồi sinh ra DB).

`Entities/NewsArticle.cs` (rút gọn):
```csharp
public partial class NewsArticle
{
    public string NewsArticleId { get; set; } = null!;
    public string? NewsTitle { get; set; }
    public string Headline { get; set; } = null!;
    public DateTime? CreatedDate { get; set; }
    public string? NewsContent { get; set; }
    public string? NewsSource { get; set; }
    public short? CategoryId { get; set; }
    public bool? NewsStatus { get; set; }
    public short? CreatedById { get; set; }
    public short? UpdatedById { get; set; }
    public DateTime? ModifiedDate { get; set; }

    public virtual Category? Category { get; set; }       // navigation 1-1
    public virtual SystemAccount? CreatedBy { get; set; }  // navigation 1-1
    public virtual ICollection<Tag> Tags { get; set; } = new List<Tag>(); // navigation N-N
}
```

- `Tags` là **navigation property dạng many-to-many** — EF Core tự nhận ra bảng `NewsTag` chỉ có 2 cột khóa ngoại (không có cột dữ liệu thêm) nên sinh ra **skip-navigation** thẳng từ `NewsArticle` sang `Tag`, không cần ta tự viết entity `NewsTag` riêng.
- Một chỗ mình **sửa tay sau khi scaffold**: EF tự đặt tên `Category.InverseParentCategory` cho danh sách con — mình đổi thành `Category.Children` cho dễ đọc (chỉ là rename, không đổi logic).

#### 2.1.2 Generic Repository (Repository Pattern)

`Repositories/IGenericRepository.cs`:
```csharp
public interface IGenericRepository<TEntity, TKey> where TEntity : class
{
    IQueryable<TEntity> Query();
    Task<TEntity?> GetByIdAsync(TKey id);
    Task<List<TEntity>> GetAllAsync();
    Task<List<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate);
    Task AddAsync(TEntity entity);
    void Update(TEntity entity);
    void Remove(TEntity entity);
}
```

`Repositories/GenericRepository.cs`:
```csharp
public class GenericRepository<TEntity, TKey> : IGenericRepository<TEntity, TKey> where TEntity : class
{
    protected readonly FUNewsManagementContext Context;
    protected readonly DbSet<TEntity> DbSet;

    public GenericRepository(FUNewsManagementContext context)
    {
        Context = context;
        DbSet = context.Set<TEntity>();
    }

    public IQueryable<TEntity> Query() => DbSet.AsQueryable();
    public async Task<TEntity?> GetByIdAsync(TKey id) => await DbSet.FindAsync(id);
    public async Task<List<TEntity>> GetAllAsync() => await DbSet.ToListAsync();
    public async Task<List<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate) =>
        await DbSet.Where(predicate).ToListAsync();
    public async Task AddAsync(TEntity entity) => await DbSet.AddAsync(entity);
    public void Update(TEntity entity) => DbSet.Update(entity);
    public void Remove(TEntity entity) => DbSet.Remove(entity);
}
```

**Vì sao có lớp Generic này?** 4 entity (`Category`, `Tag`, `NewsArticle`, `SystemAccount`) đều cần CRUD cơ bản giống nhau (Add/Update/Remove/GetById/GetAll). Viết 1 lần ở generic, rồi 4 repository cụ thể **kế thừa** và chỉ thêm method đặc thù của riêng nó — tránh lặp code.

#### 2.1.3 Repository cụ thể — ví dụ `NewsArticleRepository`

```csharp
public interface INewsArticleRepository : IGenericRepository<NewsArticle, string>
{
    Task<NewsArticle?> GetWithDetailsAsync(string id);
    IQueryable<NewsArticle> QueryWithDetails();
    Task<string> GetNextIdAsync();
}

public class NewsArticleRepository : GenericRepository<NewsArticle, string>, INewsArticleRepository
{
    public NewsArticleRepository(FUNewsManagementContext context) : base(context) { }

    public IQueryable<NewsArticle> QueryWithDetails() =>
        DbSet.Include(n => n.Category)
             .Include(n => n.CreatedBy)
             .Include(n => n.Tags)
             .AsQueryable();

    public async Task<NewsArticle?> GetWithDetailsAsync(string id) =>
        await QueryWithDetails().FirstOrDefaultAsync(n => n.NewsArticleId == id);

    public async Task<string> GetNextIdAsync()
    {
        var ids = await DbSet.Select(n => n.NewsArticleId).ToListAsync();
        var maxNumeric = ids.Select(i => int.TryParse(i, out var v) ? v : 0).DefaultIfEmpty(0).Max();
        return (maxNumeric + 1).ToString();
    }
}
```

- `QueryWithDetails()` dùng `.Include(...)` để **eager-load** (nạp kèm) Category/CreatedBy/Tags — nếu không có `Include`, khi đọc `news.Category.CategoryName` ở tầng trên sẽ bị `null` vì EF Core không tự nạp navigation property trừ khi được yêu cầu.
- `GetNextIdAsync()`: vì `NewsArticleID` là `nvarchar(20)` (không phải `IDENTITY` tự tăng) — đề chọn cách đặt ID là chuỗi số tự quản lý tay, nên ta tự tìm số lớn nhất hiện có rồi +1.
- ⚠️ **Bug đã gặp và đã sửa:** lúc đầu mình viết `DbSet.Select(...).DefaultIfEmpty(0).MaxAsync()` (gọi trực tiếp trên `IQueryable`, dịch sang SQL) — EF Core **không dịch được** cú pháp `DefaultIfEmpty` kèm tham số biến (lỗi `InvalidOperationException: could not be translated`). Cách sửa: gọi `.ToListAsync()` để lấy dữ liệu **về bộ nhớ trước**, rồi mới `.Max()` bằng LINQ-to-Objects (C# thuần, không phải SQL nữa). Đây là lỗi rất hay gặp khi mới học EF Core: không phải mọi cú pháp LINQ đều dịch được sang SQL.

#### 2.1.4 Unit of Work — gom tất cả repository + 1 lần `SaveChanges`

```csharp
public interface IUnitOfWork
{
    ICategoryRepository Categories { get; }
    INewsArticleRepository NewsArticles { get; }
    ITagRepository Tags { get; }
    ISystemAccountRepository Accounts { get; }
    Task<int> SaveChangesAsync();
}

public class UnitOfWork : IUnitOfWork
{
    private readonly FUNewsManagementContext _context;
    public UnitOfWork(FUNewsManagementContext context, ICategoryRepository categories,
        INewsArticleRepository newsArticles, ITagRepository tags, ISystemAccountRepository accounts)
    {
        _context = context;
        Categories = categories; NewsArticles = newsArticles; Tags = tags; Accounts = accounts;
    }

    public ICategoryRepository Categories { get; }
    public INewsArticleRepository NewsArticles { get; }
    public ITagRepository Tags { get; }
    public ISystemAccountRepository Accounts { get; }

    public Task<int> SaveChangesAsync() => _context.SaveChangesAsync();
}
```

**Vì sao cần Unit of Work?** Khi tạo 1 tin tức, ta vừa thêm `NewsArticle` mới, vừa có thể thêm `Tag` mới (quick-add) — đây là **2 thay đổi trên 2 repository khác nhau nhưng phải lưu cùng lúc, trong cùng 1 transaction**. Nếu mỗi repository tự gọi `SaveChanges()` riêng, sẽ có 2 transaction tách biệt → nếu transaction sau lỗi, transaction trước đã commit rồi, dữ liệu không nhất quán. Unit of Work đảm bảo **toàn bộ thay đổi trong 1 request chỉ `SaveChanges()` đúng 1 lần**.

#### 2.1.5 Đăng ký DI — `DependencyInjection.cs`

```csharp
public static class DependencyInjection
{
    public static IServiceCollection AddDataAccess(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<FUNewsManagementContext>(options => options.UseSqlServer(connectionString));
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<INewsArticleRepository, NewsArticleRepository>();
        services.AddScoped<ITagRepository, TagRepository>();
        services.AddScoped<ISystemAccountRepository, SystemAccountRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork.UnitOfWork>();
        return services;
    }
}
```
`AddScoped` = mỗi HTTP request sẽ có **1 instance riêng** của các class này (đúng với `DbContext`, vì `DbContext` không thread-safe và không nên dùng chung giữa nhiều request).

---

### 2.2 BusinessLogic layer — "biết luật chơi"

Tầng này chứa **toàn bộ rule nghiệp vụ**: khoá sửa tin sau 3 phút, không cho xoá Category/Account đang được dùng, xác thực đăng nhập (so mật khẩu / kiểm tra Google token), cache cây Category... Controller ở tầng API **không bao giờ** đụng `DbContext` trực tiếp — chỉ gọi Service.

#### 2.2.1 Mã hoá mật khẩu — `Security/PasswordHasher.cs`

```csharp
public static class PasswordHasher
{
    private const int Iterations = 100_000;
    private const int SaltSize = 16;
    private const int HashSize = 16;

    public static string Hash(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithmName.SHA256, HashSize);
        return $"{Iterations}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
    }

    public static bool Verify(string password, string? storedHash)
    {
        if (string.IsNullOrEmpty(storedHash)) return false;
        var parts = storedHash.Split('.');
        if (parts.Length != 3 || !int.TryParse(parts[0], out var iterations)) return false;

        byte[] salt, expectedHash;
        try { salt = Convert.FromBase64String(parts[1]); expectedHash = Convert.FromBase64String(parts[2]); }
        catch (FormatException) { return false; }

        var actualHash = Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, HashAlgorithmName.SHA256, expectedHash.Length);
        return CryptographicOperations.FixedTimeEquals(actualHash, expectedHash);
    }
}
```

- **Vì sao không dùng BCrypt (thư viện ngoài)?** Cột `AccountPassword` trong DB chỉ rộng `nvarchar(70)`. Hash BCrypt chuẩn dài 60 ký tự — sát mép nhưng chấp nhận được, nhưng mình muốn **không phụ thuộc NuGet ngoài** cho một thứ cơ bản như hashing, nên dùng `Rfc2898DeriveBytes` (PBKDF2) có sẵn trong .NET (`System.Security.Cryptography`), tự thiết kế format `iterations.salt.hash` rồi **chỉnh độ dài salt/hash xuống 16 byte** để chuỗi kết quả luôn đúng 56 ký tự — vừa khít cột 70 ký tự, vẫn đủ an toàn (128-bit).
- `CryptographicOperations.FixedTimeEquals` — so sánh 2 mảng byte **không bị rò thời gian xử lý** (timing-safe), chống kiểu tấn công "timing attack" đoán từng byte của hash dựa vào thời gian phản hồi.

#### 2.2.2 Đăng nhập trả về "danh tính", không còn JWT — `Services/AuthService.cs`

> **Cập nhật:** dự án ban đầu sinh JWT ở bước này (`JwtTokenService`, ký bằng `Jwt:Secret`). Vì hiện tại **chưa cần** xác thực dạng token (chỉ có đúng 1 FE gọi vào BE, không có client thứ 3 nào khác), toàn bộ `JwtTokenService`/`JwtOptions`/package `System.IdentityModel.Tokens.Jwt` đã được **xoá hẳn** để code đơn giản hơn. Xem mục [2.3.1](#231-programcs--nơi-cấu-hình-toàn-bộ-ứng-dụng) để biết cơ chế thay thế.

```csharp
public class AuthService : IAuthService
{
    public async Task<LoginResultDto?> LoginAsync(LoginRequestDto dto)
    {
        if (IsAdminEmail(dto.Email))
            return dto.Password == _adminOptions.Password ? AdminIdentity() : null;   // so với appsettings, KHÔNG so DB

        var account = await _unitOfWork.Accounts.GetByEmailAsync(dto.Email);
        if (account is null || !PasswordHasher.Verify(dto.Password, account.AccountPassword))
            return null;

        return AccountIdentity(account.AccountId, account.AccountEmail!, account.AccountName!, account.AccountRole);
    }

    private LoginResultDto AdminIdentity() => new()
    {
        AccountId = _adminOptions.AccountId,
        AccountName = _adminOptions.AccountName,
        Email = _adminOptions.Email,
        Role = RoleNames.Admin,
    };

    private static LoginResultDto AccountIdentity(short accountId, string email, string name, int? accountRole) => new()
    {
        AccountId = accountId,
        AccountName = name,
        Email = email,
        Role = RoleNames.FromAccountRole(accountRole),
    };
}
```

- `LoginResultDto` giờ **không còn field `Token`** — nó chỉ là 1 "thẻ thông tin" (accountId, tên, email, role) xác nhận "đăng nhập đúng", không kèm bằng chứng đã ký số nào cả.
- Khác biệt cốt lõi so với JWT: JWT là 1 chuỗi **tự chứng minh** (self-contained, có chữ ký) — bất kỳ ai cầm token đó gửi lên đều được tin vì chữ ký khớp. Còn ở đây, BE **tin tưởng ngay** thông tin định danh do FE gửi lên qua header thường (xem `HeaderIdentityAuthenticationHandler` mục 2.3.1) — **không có gì để xác minh cả**, nó chỉ "work" vì FE là nơi duy nhất được phép gọi BE trong môi trường này (qua CORS + thoả thuận nội bộ), không phải vì cơ chế này an toàn về mặt mật mã học.

#### 2.2.3 Xác thực Google — `Security/GoogleTokenValidator.cs`

```csharp
public record GoogleIdentity(string Email, string? Name);

public class GoogleTokenValidator : IGoogleTokenValidator
{
    private readonly GoogleAuthOptions _options;
    public GoogleTokenValidator(IOptions<GoogleAuthOptions> options) => _options = options.Value;

    public async Task<GoogleIdentity?> ValidateAsync(string idToken)
    {
        try
        {
            var settings = new GoogleJsonWebSignature.ValidationSettings { Audience = new[] { _options.ClientId } };
            var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);
            return payload.EmailVerified ? new GoogleIdentity(payload.Email, payload.Name) : null;
        }
        catch (InvalidJwtException) { return null; }
    }
}
```

- Google Identity Services (chạy ở FE, phía trình duyệt) trả về 1 `idToken` (chính là 1 JWT — nhưng **do Google ký**, không phải BE ký). BE dùng thư viện `Google.Apis.Auth` để **xác minh chữ ký của Google** (gọi tới Google để kiểm tra, hoặc kiểm tra bằng public key của Google) và lấy ra `email`/`name` đã được Google xác thực thật.
- `Audience = ClientId` đảm bảo token này **chỉ dùng cho đúng app của mình** (không bị dùng lại token cấp cho app khác).
- **Cập nhật:** trước đây hàm này chỉ trả về `email` (không tự tạo account mới — Google chỉ là cách đăng nhập thay thế cho account đã tồn tại). Theo yêu cầu mới, hàm đổi tên thành `ValidateAsync` và trả về thêm `Name` (lấy từ `payload.Name` của Google), để `AuthService` có đủ dữ liệu **tự động tạo account mới** khi email chưa từng đăng nhập — xem mục 2.3.5.

#### 2.2.4 Singleton: cache cây Category — `Caching/CategoryTreeCache.cs`

```csharp
public class CategoryTreeCache : ICategoryTreeCache
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly SemaphoreSlim _lock = new(1, 1);
    private List<CategoryNode>? _tree;

    public CategoryTreeCache(IServiceScopeFactory scopeFactory) => _scopeFactory = scopeFactory;

    public async Task<List<CategoryNode>> GetTreeAsync()
    {
        if (_tree is not null) return _tree;          // đã có cache -> trả ngay, KHÔNG hỏi DB

        await _lock.WaitAsync();                        // tránh nhiều request cùng lúc build cache song song
        try
        {
            if (_tree is not null) return _tree;        // double-check sau khi có lock

            using var scope = _scopeFactory.CreateScope();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            var categories = await unitOfWork.Categories.GetAllWithChildrenAsync();

            var nodesById = categories.ToDictionary(c => c.CategoryId, c => new CategoryNode { ... });

            var roots = new List<CategoryNode>();
            foreach (var node in nodesById.Values)
            {
                if (node.ParentCategoryId.HasValue && nodesById.TryGetValue(node.ParentCategoryId.Value, out var parent))
                    parent.Children.Add(node);
                else
                    roots.Add(node);
            }

            _tree = roots;
            return _tree;
        }
        finally { _lock.Release(); }
    }

    public void Invalidate() => _tree = null;            // gọi mỗi khi Category bị Create/Update/Delete
}
```

**Vì sao đây là Singleton và vì sao cần `IServiceScopeFactory`?**
- Class này được đăng ký `AddSingleton<ICategoryTreeCache, CategoryTreeCache>()` → **chỉ tạo 1 instance duy nhất cho toàn bộ vòng đời ứng dụng**, dùng chung cho mọi request. Đây chính là **Singleton Pattern** đề bài yêu cầu — áp dụng đúng chỗ vì menu category được đọc ở **hầu hết mọi trang** (mỗi lần render `_Layout`), nếu lần nào cũng hỏi DB thì rất lãng phí — cache giữ sẵn trong RAM, chỉ build lại khi có thay đổi.
- **Vấn đề kỹ thuật:** Singleton sống suốt đời ứng dụng, nhưng `IUnitOfWork`/`DbContext` lại đăng ký là `Scoped` (sống theo từng request) — **không thể** tiêm (`inject`) trực tiếp 1 service Scoped vào constructor của 1 service Singleton (ASP.NET Core sẽ báo lỗi runtime "Cannot consume scoped service from singleton"). Giải pháp chuẩn: tiêm `IServiceScopeFactory` (cái này luôn an toàn vì nó bản chất không có state), rồi mỗi lần cần dùng `IUnitOfWork` thì **tự tạo 1 scope tạm** (`_scopeFactory.CreateScope()`), dùng xong `using` sẽ tự dispose.
- `SemaphoreSlim` đảm bảo nếu 10 request cùng tới khi cache rỗng, chỉ 1 request được build cache (hỏi DB), 9 request còn lại chờ rồi dùng lại kết quả — tránh "cache stampede".

#### 2.2.5 Service nghiệp vụ — ví dụ đầy đủ `CategoryService.cs`

```csharp
public class CategoryService : ICategoryService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICategoryTreeCache _cache;

    public CategoryService(IUnitOfWork unitOfWork, ICategoryTreeCache cache)
    {
        _unitOfWork = unitOfWork;
        _cache = cache;
    }

    public async Task<List<CategoryTreeDto>> GetTreeAsync()
    {
        var nodes = await _cache.GetTreeAsync();
        return nodes.Select(MapNode).ToList();
    }

    public async Task<CategoryDto> CreateAsync(CategoryUpsertDto dto)
    {
        var entity = new Category { CategoryName = dto.CategoryName, CategoryDesciption = dto.CategoryDescription,
            ParentCategoryId = dto.ParentCategoryId, IsActive = dto.IsActive };
        await _unitOfWork.Categories.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();
        _cache.Invalidate();          // <-- QUAN TRỌNG: xoá cache cũ vì dữ liệu vừa đổi
        return MapEntity(entity);
    }

    public async Task DeleteAsync(short id)
    {
        var entity = await _unitOfWork.Categories.GetByIdAsync(id)
            ?? throw new EntityNotFoundException($"Category {id} not found.");

        if (await _unitOfWork.Categories.HasNewsArticlesAsync(id))
            throw new BusinessRuleException("Cannot delete a category that already has news articles.");
        if (await _unitOfWork.Categories.HasChildCategoriesAsync(id))
            throw new BusinessRuleException("Cannot delete a category that has child categories.");

        _unitOfWork.Categories.Remove(entity);
        await _unitOfWork.SaveChangesAsync();
        _cache.Invalidate();
    }
    // ... GetAllAsync / GetByIdAsync / UpdateAsync tương tự, đều Invalidate() cache sau khi sửa/xoá
}
```

- Đây chính là nơi thực hiện rule **"Category đang có tin hoặc đang có category con thì không cho xoá"** (yêu cầu của đề bài) — **không** đặt rule này ở Controller hay ở DB (constraint), mà đặt ở Service, vì đây là **logic nghiệp vụ**, dễ test, dễ đọc, và đúng kiến trúc 3-layer.
- `BusinessRuleException`/`EntityNotFoundException` là exception tự định nghĩa (xem mục 2.2.7) — Controller không bắt exception này, để 1 **middleware chung** tự convert thành mã HTTP phù hợp.

#### 2.2.6 Service phức tạp nhất — `NewsService.cs` (rule khoá sửa sau 3 phút)

```csharp
public class NewsService : INewsService
{
    public static readonly TimeSpan EditWindow = TimeSpan.FromMinutes(3);
    private readonly IUnitOfWork _unitOfWork;

    public async Task UpdateAsync(string id, NewsArticleUpsertDto dto, short updatedById, bool isAdmin)
    {
        var entity = await _unitOfWork.NewsArticles.GetWithDetailsAsync(id)
            ?? throw new EntityNotFoundException($"News article {id} not found.");

        if (!isAdmin && !CanEditNow(entity.CreatedDate, isAdmin: false))
            throw new ForbiddenOperationException(
                $"This article can no longer be edited; the {EditWindow.TotalMinutes:0}-minute edit window has elapsed.");

        entity.NewsTitle = dto.NewsTitle;
        // ... cập nhật các field khác
        entity.UpdatedById = updatedById;
        entity.ModifiedDate = DateTime.UtcNow;

        entity.Tags.Clear();                 // xoá hết liên kết tag cũ
        await AttachTagsAsync(entity, dto);   // gắn lại tag mới (cả tag có sẵn + tag tự tạo)

        _unitOfWork.NewsArticles.Update(entity);
        await _unitOfWork.SaveChangesAsync();
    }

    public static bool CanEditNow(DateTime? createdDate, bool isAdmin)
    {
        if (isAdmin) return true;                                   // Admin luôn được sửa, không giới hạn
        if (createdDate is null) return false;
        return DateTime.UtcNow - createdDate.Value <= EditWindow;    // Staff: phải còn trong 3 phút
    }
}
```

- **Quy tắc nghiệp vụ chính** nằm gọn trong 1 method tĩnh `CanEditNow` — vừa dùng để **chặn** request sửa trái phép (`UpdateAsync` gọi nó để quyết định có throw exception hay không), vừa dùng để **gắn cờ** `CanEdit` vào DTO trả về cho FE (để FE biết hiện nút Edit hay làm mờ nó đi). Dùng *cùng 1 hàm* cho cả 2 chỗ → đảm bảo logic luôn đồng bộ, không có chỗ làm khác chỗ.
- ⚠️ **Lưu ý bảo mật/thiết kế quan trọng:** FE có thể tự ẩn nút Edit dựa vào `CanEdit`, nhưng đó chỉ là UX — **BE vẫn luôn kiểm tra lại** trong `UpdateAsync` (server luôn là nguồn sự thật cuối cùng — *never trust the client*). Nếu ai đó gọi trực tiếp API bằng Postman để bypass FE, BE vẫn chặn đúng.

`DeleteAsync` — **bug thật đã gặp khi test và đã tự sửa**:

```csharp
public async Task DeleteAsync(string id)
{
    var entity = await _unitOfWork.NewsArticles.GetWithDetailsAsync(id)   // PHẢI load kèm Tags
        ?? throw new EntityNotFoundException($"News article {id} not found.");

    entity.Tags.Clear();          // gỡ hết liên kết NewsTag trước
    _unitOfWork.NewsArticles.Remove(entity);
    await _unitOfWork.SaveChangesAsync();
}
```

> **Câu chuyện lỗi thật đã xảy ra lúc test:** Ban đầu code chỉ có `GetByIdAsync` (không kèm Tags) rồi `Remove` luôn → SQL Server trả lỗi `"The DELETE statement conflicted with the REFERENCE constraint FK_NewsTag_NewsArticle"`. Lý do: bảng `NewsTag` (bảng nối N-N) có khoá ngoại trỏ tới `NewsArticle`, và khoá ngoại này **không có `ON DELETE CASCADE`** (xem schema gốc) → SQL Server **từ chối xoá** `NewsArticle` khi vẫn còn dòng `NewsTag` tham chiếu tới nó. Cách sửa: nạp kèm `Tags` (`GetWithDetailsAsync`), gọi `entity.Tags.Clear()` để EF Core tự sinh câu `DELETE FROM NewsTag WHERE NewsArticleID = ...` **trước**, rồi mới xoá `NewsArticle`. EF Core đủ thông minh để tự sắp đúng thứ tự câu lệnh SQL khi mình clear collection rồi save cùng 1 lần.

#### 2.2.7 Exception tự định nghĩa — `Exceptions/BusinessExceptions.cs`

```csharp
public class BusinessRuleException : Exception      // -> HTTP 409 Conflict
{ public BusinessRuleException(string message) : base(message) { } }

public class ForbiddenOperationException : Exception // -> HTTP 403 Forbidden
{ public ForbiddenOperationException(string message) : base(message) { } }

public class EntityNotFoundException : Exception      // -> HTTP 404 Not Found
{ public EntityNotFoundException(string message) : base(message) { } }
```

3 exception này **không tự bắt** ở Service hay Controller — chúng được "ném" (`throw`) tự nhiên, rồi 1 middleware ở tầng API (mục 2.3.2) bắt chung 1 lần và convert ra đúng mã HTTP + nội dung lỗi. Cách này giúp Service **gọn**, không phải viết try/catch lặp lại ở mọi action.

---

### 2.3 Api layer — "cửa ngõ HTTP"

#### 2.3.1 `Program.cs` — nơi cấu hình toàn bộ ứng dụng

```csharp
var builder = WebApplication.CreateBuilder(args);
const string CorsPolicy = "FrontEnd";

builder.Services.AddDataAccess(builder.Configuration.GetConnectionString("Default")!);
builder.Services.AddBusinessLogic(builder.Configuration);

builder.Services.AddControllers().AddOData(options =>
    options.Select().Filter().OrderBy().Expand().Count().SetMaxTop(200)
        .AddRouteComponents("odata", GetEdmModel()));

builder.Services.AddCors(options => { /* chỉ cho phép FE origin gọi sang */ });

// Không dùng JWT: tin tưởng trực tiếp các header định danh do FE gửi lên (xem dưới).
builder.Services.AddAuthentication(HeaderIdentityAuthenticationHandler.SchemeName)
    .AddScheme<AuthenticationSchemeOptions, HeaderIdentityAuthenticationHandler>(
        HeaderIdentityAuthenticationHandler.SchemeName, null);
builder.Services.AddAuthorization();

var app = builder.Build();
app.UseHttpsRedirection();
app.UseMiddleware<ExceptionHandlingMiddleware>();   // bắt exception nghiệp vụ -> mã HTTP
app.UseCors(CorsPolicy);
app.UseAuthentication();   // PHẢI đứng trước UseAuthorization
app.UseAuthorization();
app.MapControllers();
app.Run();

static IEdmModel GetEdmModel()
{
    var builder = new ODataConventionModelBuilder();
    builder.EntitySet<CategoryDto>("Categories").EntityType.HasKey(c => c.CategoryId);
    builder.EntitySet<TagDto>("Tags").EntityType.HasKey(t => t.TagId);
    builder.EntitySet<NewsArticleDto>("NewsArticles").EntityType.HasKey(n => n.NewsArticleId);
    return builder.GetEdmModel();
}
```

- **Thứ tự middleware rất quan trọng**: `UseAuthentication()` phải chạy **trước** `UseAuthorization()` — vì Authorization cần biết "user này là ai" (do Authentication xác định) trước khi quyết định "user này có được phép vào route này không".
- **`GetEdmModel()`** — đây là phần "khai báo OData": OData không tự đoán được model như Web API thường, mà cần 1 "Entity Data Model" (EDM) mô tả rõ entity nào có khoá gì. Vì DTO của mình tên có hậu tố `Dto` (VD `CategoryDto`) nên quy ước đặt tên tự động của OData (tìm property tên `Id` hoặc `<TênClass>Id`) **không khớp** → phải khai báo `HasKey(...)` tay cho cả 3 entity.
- `.Select().Filter().OrderBy().Expand().Count()` = bật các tính năng OData query: `$select`, `$filter`, `$orderby`, `$expand`, `$count`. `.SetMaxTop(200)` chặn client lấy quá nhiều dữ liệu 1 lần (`$top=100000` chẳng hạn).

**Không còn JWT — `HeaderIdentityAuthenticationHandler.cs`:**

```csharp
public class HeaderIdentityAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public const string SchemeName = "HeaderIdentity";
    public const string AccountIdHeader = "X-Account-Id";
    public const string AccountNameHeader = "X-Account-Name";
    public const string AccountEmailHeader = "X-Account-Email";
    public const string AccountRoleHeader = "X-Account-Role";

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var accountId = Request.Headers[AccountIdHeader].ToString();
        if (string.IsNullOrEmpty(accountId))
            return Task.FromResult(AuthenticateResult.NoResult());   // không có header -> coi như chưa đăng nhập

        var claims = new List<Claim> { new(ClaimTypes.NameIdentifier, accountId) };
        // ... đọc thêm Name/Email/Role từ header nếu có, add vào claims

        var identity = new ClaimsIdentity(claims, SchemeName);
        var ticket = new AuthenticationTicket(new ClaimsPrincipal(identity), SchemeName);
        return Task.FromResult(AuthenticateResult.Success(ticket));   // LUÔN thành công nếu có header, KHÔNG kiểm tra gì cả
    }
}
```

- `AuthenticationHandler<TOptions>` là lớp nền tảng mà cả `JwtBearerHandler`, `CookieAuthenticationHandler` (FE) đều kế thừa — nhiệm vụ duy nhất là: nhìn vào request, trả về 1 `ClaimsPrincipal` (Authentication) hoặc `NoResult()`/`Fail(...)`. Việc "có cho vào route hay không" (Authorization, `[Authorize(Roles=...)]`) là bước **sau**, không thuộc trách nhiệm của handler này.
- **Đây chính là điểm khác biệt an toàn so với JWT**: `JwtBearerHandler` xác minh **chữ ký số** trước khi tin token; handler này **không xác minh gì cả** — hễ thấy header `X-Account-Id` là tin luôn role/email đi kèm. Bất kỳ client nào tự gửi `X-Account-Role: Admin` sẽ được vào thẳng route Admin.
- **Vì sao vẫn chấp nhận được trong dự án này:** BE chỉ lắng nghe trên localhost, CORS chỉ cho phép đúng origin của FE, và FE là **nơi duy nhất** phát sinh các header này (do `IdentityForwardingHandler` ở FE tự gắn vào, dựa trên cookie đăng nhập của chính FE — xem mục 3.1). Nói cách khác, "vòng an toàn" được đẩy hết về phía cookie login của FE; BE đơn giản là tin tưởng tuyệt đối FE. **Đây KHÔNG phải kiến trúc nên dùng nếu BE có thể bị gọi trực tiếp từ một client không tin cậy** (Postman của người lạ, app mobile, FE khác...) — lúc đó phải quay lại JWT (hoặc cookie dùng chung giữa FE/BE) để có chữ ký/khoá bí mật thật.

#### 2.3.2 Middleware xử lý exception tập trung — `Middleware/ExceptionHandlingMiddleware.cs`

```csharp
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    public ExceptionHandlingMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        try { await _next(context); }
        catch (EntityNotFoundException ex) { await WriteProblem(context, 404, ex.Message); }
        catch (ForbiddenOperationException ex) { await WriteProblem(context, 403, ex.Message); }
        catch (BusinessRuleException ex) { await WriteProblem(context, 409, ex.Message); }
    }

    private static Task WriteProblem(HttpContext context, int statusCode, string message)
    {
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";
        return context.Response.WriteAsJsonAsync(new { error = message });
    }
}
```

Middleware trong ASP.NET Core hoạt động như "vòng lồng nhau" — gọi `_next(context)` để chạy tiếp middleware/Controller phía sau; nếu phía sau ném exception, nó "nảy" lại đây và bị `catch`. Đặt middleware này **sớm trong pipeline** (gần đầu `Program.cs`) để nó "bọc" được toàn bộ phần xử lý phía sau, kể cả lỗi xảy ra sâu trong Service.

#### 2.3.3 Controller dùng OData (chỉ để ĐỌC, công khai cho Guest) — `Controllers/OData/NewsArticlesController.cs`

```csharp
[AllowAnonymous]
public class NewsArticlesController : ODataController
{
    private readonly INewsService _newsService;
    public NewsArticlesController(INewsService newsService) => _newsService = newsService;

    [EnableQuery]
    public async Task<ActionResult<IQueryable<NewsArticleDto>>> Get()
    {
        var news = await _newsService.GetActiveAsync(keyword: null, tagId: null);  // CHỈ tin active
        return Ok(news.AsQueryable());
    }

    [EnableQuery]
    public async Task<ActionResult<NewsArticleDto>> Get([FromRoute] string key)
    {
        var article = await _newsService.GetActiveByIdAsync(key);
        return article is null ? NotFound() : Ok(article);
    }
}
```

- `[EnableQuery]` là "phép màu" của OData: nó nhận `IQueryable<NewsArticleDto>` ta trả về, rồi **tự áp dụng** các tham số `$filter`, `$orderby`, `$expand`... có trong URL **lên trên chính `IQueryable` đó**, trước khi serialize ra JSON trả về client. Nhờ vậy FE chỉ cần gọi:
  ```
  GET /odata/NewsArticles?$filter=contains(NewsTitle,'AI')&$expand=Tags
  GET /odata/NewsArticles?$filter=Tags/any(t: t/TagName eq 'Education')   <-- tìm theo SEO tag
  ```
  mà BE **không cần viết thêm code** xử lý từng kiểu filter — đây chính là lý do đề bài (và yêu cầu "SEO tag") chọn OData.
- **An toàn dữ liệu:** dù bật `[EnableQuery]` cho phép FE tự filter linh hoạt, nhưng **trước khi** đưa vào `IQueryable`, code đã gọi `GetActiveAsync(...)` lọc **chỉ tin có `NewsStatus = true`** — Guest dù filter kiểu gì cũng không bao giờ thấy được tin chưa duyệt/đã ẩn.

#### 2.3.4 Controller REST thường (có rule nghiệp vụ, cần đăng nhập) — `Controllers/NewsController.cs`

```csharp
[ApiController]
[Route("api/news")]
[Authorize(Roles = RoleNames.StaffOrAdmin)]
public class NewsController : ControllerBase
{
    private readonly INewsService _newsService;

    [HttpGet("manage")]
    public async Task<ActionResult<List<NewsArticleDto>>> GetForManagement(
        [FromQuery] string? keyword, [FromQuery] int? tagId, [FromQuery] bool mineOnly = false)
    {
        short? createdById = mineOnly ? this.CurrentAccountId() : null;
        var news = await _newsService.GetForManagementAsync(keyword, tagId, createdById, this.IsAdmin());
        return Ok(news);
    }

    [HttpPost]
    public async Task<ActionResult<NewsArticleDto>> Create(NewsArticleUpsertDto dto)
    {
        var created = await _newsService.CreateAsync(dto, this.CurrentAccountId());
        return CreatedAtAction(nameof(GetById), new { id = created.NewsArticleId }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, NewsArticleUpsertDto dto)
    {
        await _newsService.UpdateAsync(id, dto, this.CurrentAccountId(), this.IsAdmin());
        return NoContent();
    }
    // ...
}
```

`this.CurrentAccountId()` và `this.IsAdmin()` là 2 **extension method** (mở rộng) viết riêng trong `Common/ControllerExtensions.cs`:

```csharp
public static class ControllerExtensions
{
    public static short CurrentAccountId(this ControllerBase controller) =>
        short.Parse(controller.User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    public static bool IsAdmin(this ControllerBase controller) =>
        controller.User.IsInRole(RoleNames.Admin);
}
```

- `controller.User` chính là `ClaimsPrincipal` mà middleware `UseAuthentication()` đã dựng lên từ các header `X-Account-*` (qua `HeaderIdentityAuthenticationHandler`, mục 2.3.1) và gắn sẵn vào `HttpContext` — Controller chỉ cần đọc claim ra, không quan tâm claim đó đến từ JWT hay từ header thường (code Controller **không đổi gì** khi đổi cơ chế xác thực, vì cả 2 đều cuối cùng tạo ra `ClaimsPrincipal`).
- Đây cũng là minh chứng rõ nhất cho thiết kế "BE luôn tự kiểm tra lại": dù FE gửi `mineOnly=true`, BE **tự lấy accountId từ `User` đã xác thực** (`CurrentAccountId()`) chứ không tin tham số accountId nào gửi trực tiếp trong query string — tránh user A giả mạo xem lịch sử của user B. (Lưu ý: với cơ chế header-trust hiện tại, claim này **vẫn có thể bị giả mạo** nếu ai đó gọi thẳng vào BE thay vì qua FE — xem cảnh báo an toàn ở mục 2.3.1.)

#### 2.3.5 Controller cho Auth — `Controllers/AuthController.cs`

```csharp
[ApiController]
[Route("api/auth")]
[AllowAnonymous]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    [HttpPost("login")]
    public async Task<ActionResult<LoginResultDto>> Login(LoginRequestDto dto)
    {
        var result = await _authService.LoginAsync(dto);
        return result is null ? Unauthorized(new { error = "Invalid email or password." }) : Ok(result);
    }

    [HttpPost("google")]
    public async Task<ActionResult<LoginResultDto>> GoogleLogin(GoogleLoginRequestDto dto)
    {
        var result = await _authService.GoogleLoginAsync(dto);
        return result is null
            ? Unauthorized(new { error = "Invalid Google token or no matching account for this email." })
            : Ok(result);
    }
}
```

Và logic thật nằm ở `AuthService` (tầng BusinessLogic):

```csharp
public async Task<LoginResultDto?> LoginAsync(LoginRequestDto dto)
{
    if (IsAdminEmail(dto.Email))
        return dto.Password == _adminOptions.Password ? AdminIdentity() : null;  // so với appsettings, KHÔNG so DB

    var account = await _unitOfWork.Accounts.GetByEmailAsync(dto.Email);
    if (account is null || !PasswordHasher.Verify(dto.Password, account.AccountPassword))
        return null;

    return AccountIdentity(account.AccountId, account.AccountEmail!, account.AccountName!, account.AccountRole);
}

public async Task<LoginResultDto?> GoogleLoginAsync(GoogleLoginRequestDto dto)
{
    var google = await _googleTokenValidator.ValidateAsync(dto.IdToken);   // Google xác thực ai, KHÔNG cần password
    if (google is null) return null;

    if (IsAdminEmail(google.Email)) return AdminIdentity();

    var account = await _unitOfWork.Accounts.GetByEmailAsync(google.Email);
    if (account is null)
    {
        // CẬP NHẬT: email chưa từng có account -> tự tạo account Staff mới (trước đây trả null ở đây)
        account = new SystemAccount
        {
            AccountId = await _unitOfWork.Accounts.GetNextIdAsync(),
            AccountName = string.IsNullOrWhiteSpace(google.Name) ? google.Email : google.Name,
            AccountEmail = google.Email,
            AccountRole = (int)AccountRole.Staff,
            AccountPassword = null,   // tài khoản tạo từ Google, chưa có mật khẩu nội bộ
        };
        await _unitOfWork.Accounts.AddAsync(account);
        await _unitOfWork.SaveChangesAsync();
    }

    return AccountIdentity(account.AccountId, account.AccountEmail!, account.AccountName!, account.AccountRole);
}
```

Sơ đồ tư duy luồng đăng nhập (đã bỏ bước sinh JWT — chỉ còn trả về thông tin định danh thuần):

```
Đăng nhập thường:  Email+Password ─► so khớp PasswordHasher.Verify ─► trả LoginResultDto (accountId/tên/email/role)
Đăng nhập Google:  idToken (Google ký) ─► GoogleTokenValidator xác minh ─► lấy email+tên
                   ─► tìm account theo email ─► CÓ rồi: dùng luôn / CHƯA có: tự tạo account Staff mới
                   (không cần kiểm tra password vì Google đã xác thực identity rồi) ─► trả LoginResultDto
```

`LoginResultDto` này chỉ là dữ liệu thô gửi về cho FE qua HTTP thường (không ký số) — FE tự chịu trách nhiệm "nhớ" ai đã đăng nhập bằng cookie của chính mình (mục 3.1, 3.2), rồi mỗi lần gọi lại BE thì gửi lại đúng các trường này qua header để `HeaderIdentityAuthenticationHandler` tin tưởng.

**Vì sao đổi từ "chỉ login được nếu account đã tồn tại" sang "tự tạo account mới"?** Theo yêu cầu mới của người dùng: bất kỳ ai đăng nhập Google thành công (Google đã xác thực email thật) đều **tự động được cấp 1 account Staff** nếu email đó chưa từng có trong `SystemAccount`. Account mới này:
- Có `AccountRole = Staff` theo mặc định (không tự tạo Admin qua Google, tránh leo thang quyền) — Admin vẫn có thể vào `Account Management` để đổi role sau.
- Có `AccountPassword = null` — vì người này chưa từng đặt mật khẩu nội bộ, họ chỉ đăng nhập được qua Google cho tới khi Admin gán mật khẩu (hoặc tự đổi mật khẩu nếu hệ thống có tính năng đó).
- Dùng `_unitOfWork.Accounts.GetNextIdAsync()` để tự sinh `AccountId` tiếp theo — **giống cách `AccountService.CreateAsync` (Admin tạo tay) đang làm**, đảm bảo không đụng ID đã có.

#### 2.3.6 Các Controller còn lại của BE (CRUD thuần, không có rule nghiệp vụ đặc biệt)

Ngoài `NewsController`/`AuthController`/`OData/NewsArticlesController` đã giải thích chi tiết ở trên, BE còn 1 nhóm Controller "CRUD chuẩn" — đều theo khuôn mẫu giống nhau (`[ApiController]` + gọi đúng 1 Service + không đụng `DbContext`), nên chỉ tóm tắt route/quyền thay vì lặp lại code:

| Controller | Route | Quyền | Việc làm |
|---|---|---|---|
| `Controllers/AccountsController.cs` | `api/accounts` | `[Authorize(Roles = Admin)]` toàn bộ | CRUD `SystemAccount` — `AccountService` đã chặn xoá/sửa account Admin và account đang có tin đã viết (mục 2.2.5-style rule, xem `AccountService.cs`). |
| `Controllers/CategoriesController.cs` | `api/categories` | Đọc: `[AllowAnonymous]` (Guest cần thấy menu); Ghi: `Staff,Admin` | Trùng dữ liệu với `OData/CategoriesController` nhưng là REST thuần — dùng cho các action Create/Update/Delete (OData ở đây chỉ cấu hình cho đọc). Có thêm route riêng `GET api/categories/tree` trả cây đã build sẵn (gọi `ICategoryService.GetTreeAsync()`, tức đi qua Singleton cache — mục 2.2.4). |
| `Controllers/TagsController.cs` | `api/tags` | Đọc: `[AllowAnonymous]`; Ghi: `Admin`; riêng `POST api/tags/get-or-create`: `Staff,Admin` | CRUD Tag chuẩn cho Admin. Endpoint `get-or-create` riêng cho Staff: khi soạn tin và gõ tên tag mới chưa có, FE gọi route này để "tìm tag theo tên, nếu chưa có thì tạo luôn" — phục vụ tính năng tự thêm SEO tag ngay trong lúc viết tin (xem `NewsService.AttachTagsAsync`, mục 2.2.6, làm việc tương tự ngay trong lúc lưu tin). |
| `Controllers/ReportsController.cs` | `api/reports` | `[Authorize(Roles = Admin)]` | 1 action duy nhất `GET api/reports?startDate=...&endDate=...` — gọi `INewsService.GetReportAsync(start, end)`, trả danh sách tin trong khoảng ngày, **sắp giảm dần theo `CreatedDate`** đúng yêu cầu đề bài. |
| `Controllers/OData/CategoriesController.cs` | `odata/Categories` | `[AllowAnonymous]` | Bản OData read-only của Category — cho phép FE dùng `$filter`/`$expand` khi cần (vd lọc `IsActive eq true`), khác với route REST `api/categories` (dùng cho menu phía Guest + dropdown chọn Category lúc tạo tin). |
| `Controllers/OData/TagsController.cs` | `odata/Tags` | `[AllowAnonymous]` | Bản OData read-only của Tag — phục vụ ô chọn/tìm tag (autocomplete) cần `$filter=contains(TagName,'...')` mà không cần thêm action riêng ở `TagsController` REST. |

**Vì sao có cả route REST (`api/categories`, `api/tags`) và route OData (`odata/Categories`, `odata/Tags`) cho cùng 1 entity?** OData chỉ thật sự cần cho phần **đọc, lọc linh hoạt** (Guest tìm tin theo tag, FE tự ghép `$filter` động). Còn các hành động Create/Update/Delete (có ràng buộc nghiệp vụ như "không xoá Category đang có tin") thì viết Controller REST thuần để dễ kiểm soát luồng (gọi đúng Service, đúng `[Authorize(Roles=...)]`) hơn là cố nhồi vào quy ước OData hành động ghi.

---

## 3. FRONTEND — chi tiết theo từng phần

### 3.1 FE "nhớ" ai đăng nhập bằng cách nào, và truyền danh tính đó sang BE ra sao?

FE là ứng dụng MVC cổ điển — trình duyệt giữ **cookie**, không tự giữ token trong header như app SPA. Cơ chế hiện tại (đã bỏ JWT):

1. User đăng nhập ở FE → FE gọi BE `/api/auth/login` → BE trả về `LoginResultDto` (accountId, tên, email, role — **không có token nào cả**).
2. FE lưu các thông tin đó vào **cookie đăng nhập của chính FE** (Cookie Authentication chuẩn của ASP.NET Core MVC, mục 3.2).
3. Mỗi khi FE cần gọi API của BE (lấy danh sách tin, tạo tin...), 1 `DelegatingHandler` sẽ **đọc lại các claim đó từ cookie hiện tại** và gắn thành các header thường (`X-Account-Id`, `X-Account-Name`, `X-Account-Email`, `X-Account-Role`) vào request gửi sang BE.

```csharp
// Common/IdentityForwardingHandler.cs
public class IdentityForwardingHandler : DelegatingHandler
{
    public const string AccountIdHeader = "X-Account-Id";
    public const string AccountNameHeader = "X-Account-Name";
    public const string AccountEmailHeader = "X-Account-Email";
    public const string AccountRoleHeader = "X-Account-Role";

    private readonly IHttpContextAccessor _httpContextAccessor;
    public IdentityForwardingHandler(IHttpContextAccessor httpContextAccessor) => _httpContextAccessor = httpContextAccessor;

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var user = _httpContextAccessor.HttpContext?.User;
        var accountId = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!string.IsNullOrEmpty(accountId))
        {
            request.Headers.Add(AccountIdHeader, accountId);
            // ... AddIfPresent tương tự cho Name / Email / Role nếu có
        }

        return base.SendAsync(request, cancellationToken);   // KHÔNG có Authorization: Bearer nào ở đây
    }
}
```

- `DelegatingHandler` là 1 "lớp lọc" gắn vào pipeline của `HttpClient` — mọi request gửi qua `HttpClient` này đều chạy qua `SendAsync` này trước khi thực sự ra network. Đây là chỗ **chỉ viết 1 lần**, dùng chung cho tất cả các API Client (`NewsApiClient`, `CategoryApiClient`,...) mà không cần lặp code gắn header ở từng nơi.
- **So với bản JWT trước đây**, chỗ khác biệt duy nhất là: trước kia handler này gắn `Authorization: Bearer <jwt>` (1 chuỗi đã được BE ký), giờ nó gắn thẳng 4 header thô không có chữ ký. Về mặt kiến trúc pipeline (`AddHttpMessageHandler<T>()`) thì **giống hệt nhau** — chỉ đổi *nội dung* header gắn vào.
- **Cảnh báo an toàn (đã thống nhất với người dùng):** vì các header này không có chữ ký, **bất kỳ ai gọi trực tiếp vào BE** (bỏ qua FE) đều có thể tự ghi `X-Account-Role: Admin` để giả mạo quyền Admin. Cách này chỉ chấp nhận được vì đây là **bản nội bộ/demo**, BE không expose ra ngoài và FE là caller duy nhất. Nếu sau này cần mở BE cho client khác hoặc deploy ra ngoài, phải quay lại JWT hoặc 1 cơ chế có chữ ký/khoá bí mật thật.

Đăng ký ở `Program.cs` (FE):
```csharp
builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<IdentityForwardingHandler>();

builder.Services.AddHttpClient<INewsApiClient, NewsApiClient>(c => c.BaseAddress = new Uri(apiBaseUrl))
    .AddHttpMessageHandler<IdentityForwardingHandler>();   // <-- gắn handler vào pipeline của HttpClient này
```

### 3.2 Đăng nhập (cả 2 kiểu) ở `Controllers/AccountController.cs`

```csharp
[AllowAnonymous]
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Login(LoginRequestDto model, string? returnUrl = null)
{
    if (!ModelState.IsValid) return View(model);

    var result = await _authApiClient.LoginAsync(model.Email, model.Password);
    if (result is null) { ModelState.AddModelError(string.Empty, "Invalid email or password."); return View(model); }

    await SignInAsync(result);
    return RedirectToLocal(returnUrl);
}

[AllowAnonymous]
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> GoogleLogin(string credential, string? returnUrl = null)
{
    var result = await _authApiClient.GoogleLoginAsync(credential);
    if (result is null) { TempData["LoginError"] = "..."; return RedirectToAction(nameof(Login), new { returnUrl }); }

    await SignInAsync(result);
    return RedirectToLocal(returnUrl);
}

private async Task SignInAsync(LoginResultDto result)
{
    var claims = new List<Claim>
    {
        new(ClaimTypes.NameIdentifier, result.AccountId.ToString()),
        new(ClaimTypes.Name, result.AccountName),
        new(ClaimTypes.Email, result.Email),
        new(ClaimTypes.Role, result.Role),
        // không còn claim JWT nào để nhồi vào đây nữa
    };
    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
}
```

`HttpContext.SignInAsync(...)` là API của **Cookie Authentication** trong ASP.NET Core — nó tự mã hoá toàn bộ `ClaimsPrincipal` thành 1 cookie, gửi về cho trình duyệt qua header `Set-Cookie`. Những request sau, trình duyệt tự gửi cookie này lên lại, middleware `UseAuthentication()` của FE tự giải mã ra `User` — và chính các claim này (NameIdentifier/Name/Email/Role) là nguồn mà `IdentityForwardingHandler` (mục 3.1) đọc lại để gắn vào header `X-Account-*` gửi sang BE.

Phía giao diện, nút "Sign in with Google" dùng **Google Identity Services** (thư viện JS của Google, không phải code mình tự viết luồng OAuth redirect):

```html
<!-- Views/Account/Login.cshtml -->
<div id="g_id_onload" data-client_id="@ViewBag.GoogleClientId" data-callback="handleGoogleCredentialResponse"></div>
<div class="g_id_signin" data-type="standard"></div>

<form id="googleLoginForm" asp-action="GoogleLogin" method="post">
    <input type="hidden" name="credential" id="googleCredential" />
</form>

<script src="https://accounts.google.com/gsi/client" async defer></script>
<script>
    function handleGoogleCredentialResponse(response) {
        document.getElementById('googleCredential').value = response.credential;  // idToken Google trả về
        document.getElementById('googleLoginForm').submit();                       // gửi lên FE Controller
    }
</script>
```

Luồng: Google hiện nút đăng nhập → user bấm → Google trả về 1 `credential` (chính là idToken) ngay tại trình duyệt (không cần redirect qua server) → JS nhúng nó vào form ẩn và **submit luôn** → FE `AccountController.GoogleLogin` nhận, forward sang BE `/api/auth/google` để xác minh.

### 3.3 API Client — lớp "dịch" giữa MVC Controller và HTTP gọi BE

Tất cả API Client đều theo khuôn giống nhau: interface + class implement, dùng `HttpClient` đã được cấu hình `BaseAddress`. Ví dụ điển hình `NewsApiClient` (phần build query OData):

```csharp
public async Task<List<NewsArticleDto>> GetActiveAsync(string? keyword, int? tagId, short? categoryId = null)
{
    var filters = new List<string>();

    if (!string.IsNullOrWhiteSpace(keyword))
    {
        var escaped = EscapeODataString(keyword);   // escape dấu nháy đơn cho an toàn cú pháp OData
        filters.Add($"(contains(NewsTitle,'{escaped}') or contains(Headline,'{escaped}') " +
                     $"or Tags/any(t: contains(t/TagName,'{escaped}')))");
    }
    if (tagId.HasValue) filters.Add($"Tags/any(t: t/TagId eq {tagId.Value})");
    if (categoryId.HasValue) filters.Add($"CategoryId eq {categoryId.Value}");

    var query = new StringBuilder("odata/NewsArticles?$expand=Tags&$orderby=CreatedDate desc");
    if (filters.Count > 0)
        query.Append("&$filter=").Append(Uri.EscapeDataString(string.Join(" and ", filters)));

    var result = await _httpClient.GetFromJsonAsync<ODataResponse<NewsArticleDto>>(query.ToString(), JsonOptions.Default);
    return result?.Value ?? new();
}
```

- `Tags/any(t: ...)` là cú pháp **lambda của OData** — nghĩa là "tồn tại ít nhất 1 phần tử `t` trong collection `Tags` thoả điều kiện" — chính là cách viết OData cho "tìm theo SEO tag".
- `Uri.EscapeDataString(...)` mã hoá chuỗi `$filter` để các ký tự đặc biệt (dấu cách, dấu nháy, `/`) không phá vỡ URL.
- `ODataResponse<T>` là 1 wrapper nhỏ:
  ```csharp
  public class ODataResponse<T>
  {
      [JsonPropertyName("value")]
      public List<T> Value { get; set; } = new();
  }
  ```
  Vì OData luôn bọc kết quả trong `{ "value": [...] }` (kèm `@odata.context`), không trả thẳng `[...]` như REST thường — nên cần class riêng để deserialize đúng.

`Common/HttpResponseExtensions.cs` — chuẩn hoá xử lý lỗi cho **tất cả** API client:

```csharp
public static async Task EnsureApiSuccessAsync(this HttpResponseMessage response)
{
    if (response.IsSuccessStatusCode) return;

    var message = $"Request failed with status {(int)response.StatusCode}.";
    try
    {
        var body = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(body);
        if (doc.RootElement.TryGetProperty("error", out var errorProp))
            message = errorProp.GetString() ?? message;
    }
    catch (JsonException) { /* body không phải JSON -> giữ message mặc định */ }

    throw new ApiException(response.StatusCode, message);
}
```

Đây chính là nơi "bắt" được message lỗi nghiệp vụ mà BE trả ra (`{ "error": "Cannot delete a category that already has news articles." }` — sinh ra từ `ExceptionHandlingMiddleware` ở mục 2.3.2) và ném lại thành `ApiException` ở phía FE, để FE Controller bắt và hiển thị **đúng nguyên câu lỗi nghiệp vụ** cho người dùng (không phải lỗi chung "Internal Server Error").

### 3.4 Menu nhiều cấp — `ViewComponents/CategoryMenuViewComponent.cs`

```csharp
public class CategoryMenuViewComponent : ViewComponent
{
    private readonly ICategoryApiClient _categoryApiClient;
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var tree = await _categoryApiClient.GetTreeAsync();
        return View(tree.Where(c => c.IsActive).ToList());
    }
}
```

**View Component** là 1 đơn vị UI tái sử dụng, có thể tự gọi service/API riêng — khác `Partial View` thường (partial chỉ render lại model có sẵn, không tự fetch dữ liệu được). Vì menu cần tự đi lấy cây Category mỗi lần render `_Layout`, View Component là lựa chọn đúng.

Gọi nó trong `_Layout.cshtml`:
```cshtml
@await Component.InvokeAsync("CategoryMenu")
```

Phần đệ quy (render lồng nhau) — `Views/Shared/_CategoryMenuNode.cshtml`:

```cshtml
@model CategoryTreeDto
@{
    var isTopLevel = ViewData["IsTopLevel"] as bool? ?? false;
}
@if (Model.Children.Any())
{
    <li class="@(isTopLevel ? "nav-item dropdown" : "dropdown-submenu")">
        <a class="dropdown-toggle" data-bs-toggle="dropdown">@Model.CategoryName</a>
        <ul class="dropdown-menu">
            <li><a asp-action="Index" asp-route-categoryId="@Model.CategoryId">All @Model.CategoryName</a></li>
            @foreach (var child in Model.Children)
            {
                @await Html.PartialAsync("_CategoryMenuNode", child,
                    new ViewDataDictionary(ViewData) { ["IsTopLevel"] = false })   // <-- GỌI LẠI CHÍNH NÓ
            }
        </ul>
    </li>
}
else { <li><a asp-route-categoryId="@Model.CategoryId">@Model.CategoryName</a></li> }
```

**Vì sao đây là "đệ quy" (recursion)?** Partial view này, ngay trong thân của mình, lại gọi `Html.PartialAsync("_CategoryMenuNode", child, ...)` — tức **gọi lại chính file đang chạy**, nhưng với `child` (1 cấp con) làm model mới. Vì cây Category có thể sâu bao nhiêu cấp cũng được (ở đây là 3 cấp: Academic news → Technology → .NET/Java), viết đệ quy giúp code xử lý **đúng với mọi độ sâu**, không cần biết trước cây sâu mấy cấp.

Bootstrap 5 vốn **không hỗ trợ dropdown lồng nhau** (submenu) — phải tự viết thêm CSS (`wwwroot/css/site.css`):
```css
.dropdown-submenu { position: relative; }
.dropdown-submenu > .dropdown-menu { top: 0; left: 100%; margin-top: -1px; }
.dropdown-submenu.show > .dropdown-menu { display: block; }
```
và JS (`wwwroot/js/site.js`) để bấm vào 1 submenu thì nó "xoè" sang cạnh phải mà không đóng menu cha:
```js
document.addEventListener('click', function (event) {
    const toggle = event.target.closest('.dropdown-submenu > .dropdown-toggle');
    if (!toggle) return;
    event.preventDefault(); event.stopPropagation();
    const submenu = toggle.closest('.dropdown-submenu');
    const wasOpen = submenu.classList.contains('show');
    submenu.parentElement.querySelectorAll('.dropdown-submenu.show').forEach(el => { if (el !== submenu) el.classList.remove('show'); });
    submenu.classList.toggle('show', !wasOpen);
});
```

### 3.5 CKEditor + khoá sửa 3 phút ở phía FE — `Views/News/Manage.cshtml`

```html
<button class="btn btn-sm btn-outline-secondary edit-news-btn" data-id="@news.NewsArticleId">Edit</button>
@* hoặc nếu hết hạn sửa: *@
<button class="btn btn-sm btn-outline-secondary" disabled title="3-minute edit window has elapsed">Edit</button>
```
```js
ClassicEditor.create(document.querySelector('#create-NewsContent')).then(editor => createEditor = editor);
ClassicEditor.create(document.querySelector('#edit-NewsContent')).then(editor => editEditor = editor);

document.querySelectorAll('.edit-news-btn').forEach(btn => btn.addEventListener('click', async () => {
    const id = btn.getAttribute('data-id');
    const response = await fetch('/News/GetJson/' + encodeURIComponent(id));
    const news = await response.json();

    document.getElementById('editForm').action = '/News/Edit/' + encodeURIComponent(id);
    document.getElementById('edit-Headline').value = news.headline || '';
    // ... fill các field khác
    if (editEditor) editEditor.setData(news.newsContent || '');   // bơm nội dung HTML vào CKEditor

    new bootstrap.Modal(document.getElementById('editModal')).show();
}));
```

- Vì sao chỉ **1 modal Edit chung** cho mọi dòng (không tạo N modal cho N tin)? Nếu tạo riêng 1 modal + 1 CKEditor instance cho mỗi dòng tin, trang sẽ rất nặng khi có nhiều tin (mỗi CKEditor là 1 instance JS khá tốn tài nguyên). Giải pháp: **1 modal dùng lại**, mỗi lần bấm "Edit" ở dòng nào thì `fetch('/News/GetJson/{id}')` lấy dữ liệu mới nhất của dòng đó, rồi **đổ dữ liệu vào modal** (kể cả gọi `editEditor.setData(...)` để bơm lại nội dung rich-text) trước khi mở modal lên.
- `[Route("News/GetJson/{id}")]` ở Controller chỉ là 1 API JSON nhỏ phục vụ riêng cho JS này, **không phải** trang HTML:
  ```csharp
  [HttpGet]
  public async Task<IActionResult> GetJson(string id)
  {
      var article = await _newsApiClient.GetByIdForManagementAsync(id);
      return article is null ? NotFound() : Json(article);
  }
  ```
- **Phòng vệ 2 lớp cho rule "khoá sửa sau 3 phút":** ở UI, nút Edit bị `disabled` nếu `news.CanEdit == false` (lấy từ field `CanEdit` mà BE đã tính — xem mục 2.2.6) — đây chỉ là **trải nghiệm người dùng** (UX), để họ không phải bấm rồi mới biết bị từ chối. Còn **chốt chặn thật** vẫn là BE: nếu ai cố tình gọi API `PUT /api/news/{id}` sau 3 phút (vd qua Postman, hoặc sửa JS để bỏ `disabled`), BE vẫn ném `ForbiddenOperationException` → HTTP 403.

### 3.6 Bug thật đã gặp lúc còn dùng JWT: app crash khi JWT bên trong cookie hết hạn

> **Lưu ý:** sau khi bỏ JWT (mục 2.2.2, 2.3.1, 3.1), **nguyên nhân gốc** của bug dưới đây (lệch thời gian sống giữa cookie FE và JWT BE) **không còn tồn tại nữa** — header định danh hiện tại không có khái niệm "hết hạn" riêng, nó sống/chết theo đúng cookie FE. Phần này được **giữ lại để học**, vì 2 thứ quan trọng nó để lại vẫn còn dùng nguyên trong code hiện tại: (1) thói quen luôn đi qua `GetJsonSafeAsync`/`EnsureApiSuccessAsync` thay vì gọi `GetFromJsonAsync` trực tiếp, và (2) `ApiExceptionFilter` bắt lỗi tập trung — cả hai vẫn cần thiết để xử lý gọn các lỗi 403 (sai quyền) hay 404/409 (lỗi nghiệp vụ) mà BE trả về, dù 401-do-JWT-hết-hạn không còn là 1 nguyên nhân nữa.
>
> **Hiện tượng (lúc đó):** sau khi đăng nhập và dùng app một lúc, bấm vào "Manage News" thì FE hiện trang lỗi "An unhandled exception occurred... HttpRequestException: Response status code does not indicate success: 401".

**Nguyên nhân gốc — lệch thời gian sống giữa 2 cơ chế đăng nhập:**
- Cookie đăng nhập của FE (`ExpireTimeSpan = 2 giờ`, có **Sliding Expiration** mặc định = `true`) → mỗi request mới sẽ "làm mới" lại đồng hồ 2 giờ, nên session FE có thể kéo dài rất lâu nếu user còn hoạt động.
- JWT nhúng bên trong cookie đó thì **chỉ được cấp 1 lần lúc đăng nhập** (`Jwt:ExpiryMinutes = 120`) và **không tự refresh** — quá 120 phút kể từ lúc đăng nhập, JWT chắc chắn hết hạn, bất kể FE có còn coi user là "đã đăng nhập" hay không.
- Hệ quả: FE vẫn nghĩ user còn đăng nhập (cookie hợp lệ) → cho qua `[Authorize(Roles="Staff,Admin")]` → action chạy → gọi sang BE bằng JWT đã hết hạn → BE trả `401 Unauthorized`.

**Vì sao 401 đó lại làm sập trang?** Một số API Client gọi `_httpClient.GetFromJsonAsync<T>(url)` **trực tiếp** (API có sẵn của .NET) — hàm này tự gọi `EnsureSuccessStatusCode()` bên trong, và ném `HttpRequestException` thẳng ra ngoài nếu response không phải 2xx. Exception này **không được Controller nào bắt** → ASP.NET Core hiện trang lỗi mặc định ("Developer Exception Page" ở môi trường Development).

**Cách sửa — 2 bước:**

1. **Chuẩn hoá lại toàn bộ API Client** để luôn đi qua `EnsureApiSuccessAsync()` (đã có từ trước, đọc được message lỗi `{ "error": "..." }` của BE) thay vì gọi `GetFromJsonAsync` trực tiếp. Thêm 1 hàm tiện ích chung:

   ```csharp
   // Common/HttpClientExtensions.cs
   public static class HttpClientExtensions
   {
       public static async Task<T?> GetJsonSafeAsync<T>(this HttpClient httpClient, string requestUri)
       {
           var response = await httpClient.GetAsync(requestUri);
           await response.EnsureApiSuccessAsync();   // ném ApiException (có StatusCode + message rõ ràng), không phải HttpRequestException trần
           return await response.Content.ReadFromJsonAsync<T>(JsonOptions.Default);
       }
   }
   ```
   Rồi đổi mọi `_httpClient.GetFromJsonAsync<T>(url, JsonOptions.Default)` thành `_httpClient.GetJsonSafeAsync<T>(url)` trong `CategoryApiClient`, `TagApiClient`, `NewsApiClient`, `AccountApiClient`, `ReportApiClient`.

2. **Bắt `ApiException` ở 1 nơi duy nhất** bằng 1 *Exception Filter* toàn cục (`IAsyncExceptionFilter`), thay vì rải try/catch ở từng Controller:

   ```csharp
   // Common/ApiExceptionFilter.cs
   public class ApiExceptionFilter : IAsyncExceptionFilter
   {
       public async Task OnExceptionAsync(ExceptionContext context)
       {
           if (context.Exception is not ApiException apiException) return;

           if (apiException.StatusCode == HttpStatusCode.Unauthorized)
           {
               // JWT trong cookie không còn hợp lệ -> đăng xuất sạch sẽ, đưa về trang Login
               var returnUrl = context.HttpContext.Request.Path + context.HttpContext.Request.QueryString;
               await context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
               context.Result = new RedirectToActionResult("Login", "Account", new { returnUrl, sessionExpired = true });
               context.ExceptionHandled = true;
               return;
           }

           if (apiException.StatusCode == HttpStatusCode.Forbidden)
           {
               context.Result = new RedirectToActionResult("AccessDenied", "Account", null);
               context.ExceptionHandled = true;
               return;
           }

           // Các lỗi nghiệp vụ khác (404/409/500...) -> hiện banner đỏ rồi quay lại trang trước, không crash
           var tempData = context.HttpContext.RequestServices
               .GetRequiredService<ITempDataDictionaryFactory>().GetTempData(context.HttpContext);
           tempData["Error"] = apiException.Message;

           var referer = context.HttpContext.Request.Headers.Referer.ToString();
           context.Result = string.IsNullOrEmpty(referer)
               ? new RedirectToActionResult("Index", "Home", null)
               : new RedirectResult(referer);
           context.ExceptionHandled = true;
       }
   }
   ```

   Đăng ký toàn cục trong `Program.cs`:
   ```csharp
   builder.Services.AddControllersWithViews(options => options.Filters.Add<ApiExceptionFilter>());
   ```

   *Exception Filter* trong ASP.NET Core MVC là 1 "móc" (hook) chạy **sau khi** Action ném exception nhưng **trước khi** ASP.NET Core hiện trang lỗi mặc định — đặt logic xử lý lỗi tập trung ở đây giúp **toàn bộ Controller** tự động được bảo vệ mà không cần sửa từng action.

3. Trang Login hiển thị thông báo thân thiện khi bị đá ra do hết hạn (`/Account/Login?sessionExpired=true`):
   ```csharp
   public IActionResult Login(string? returnUrl = null, bool sessionExpired = false)
   {
       if (sessionExpired) TempData["LoginError"] = "Your session has expired. Please log in again.";
       ...
   }
   ```

**Đã kiểm chứng bằng cách giả lập đúng tình huống lỗi:** đăng nhập lấy cookie hợp lệ, sau đó đổi `Jwt:Secret` của BE rồi khởi động lại BE (mọi JWT cũ lập tức bị coi là không hợp lệ — mô phỏng đúng hệ quả của việc hết hạn) → gọi lại `/News/Manage` bằng cookie cũ → kết quả: `302 Found` chuyển hướng tới `/Account/Login?returnUrl=%2FNews%2FManage&sessionExpired=True`, cookie cũ bị xoá (`Set-Cookie` với ngày hết hạn ở quá khứ), trang Login hiện đúng dòng "Your session has expired. Please log in again." — không còn trang lỗi unhandled exception nữa.

> **Bài học:** Khi 1 hệ thống có **2 cơ chế "còn hạn" độc lập** (ở đây là cookie FE và JWT BE) chạy song song, luôn phải tự hỏi: "nếu cái này hết hạn mà cái kia chưa, chuyện gì xảy ra?" — và luôn xử lý ở **lớp biên** (boundary) thấp nhất có thể (ở đây là tầng gọi HTTP ra ngoài), thay vì hy vọng nó không bao giờ xảy ra.

### 3.7 Các Controller FE còn lại (theo khuôn "popup CRUD" giống nhau)

Các Controller này đều theo đúng 1 khuôn: action gọi 1 API Client, `catch (ApiException)` rồi đẩy message lỗi vào `TempData["Error"]` (phòng hờ trường hợp `ApiExceptionFilter` ở mục 3.6 chưa kịp bắt, vd lỗi nghiệp vụ 409/404 mà Controller tự muốn xử lý gọn ngay tại action), không có view riêng cho Create/Edit — Bootstrap **modal** trên cùng 1 trang Index đóng vai trò form, đúng yêu cầu đề bài ("quản lý qua popup, không cần trang riêng"):

| Controller | Quyền | Trang/Action chính |
|---|---|---|
| `Controllers/HomeController.cs` | `[AllowAnonymous]` | `Index(keyword, tagId, categoryId)`: trang chủ Guest — danh sách tin **đang active** kèm lọc theo từ khoá/tag/category (gọi `INewsApiClient.GetActiveAsync`, chính là route OData NewsArticles ở mục 2.3.3). `Details(id)`: xem chi tiết 1 tin (404 nếu tin không active hoặc không tồn tại). |
| `Controllers/AccountController.cs` (phần quản lý account, khác phần Login đã nói ở mục 3.2) | `[Authorize(Roles = "Admin")]` cho `Index`/`Create`/`Edit`/`Delete` | Cùng khuôn modal-CRUD: `Index()` liệt kê toàn bộ account (trừ Admin ảo từ appsettings — `AccountService.GetAllAsync` đã tự lọc ở BE, mục 2.2.5-style); `Create`/`Edit`/`Delete` gọi `IAccountApiClient` rồi quay lại `Index`, lỗi nghiệp vụ (vd "không xoá được account đã viết tin") hiện qua `TempData["Error"]`. |
| `Controllers/CategoryController.cs` | `[Authorize(Roles = "Staff,Admin")]` | `Index()` liệt kê toàn bộ Category (kể cả không active) cho Staff/Admin quản lý; `Create`/`Edit`/`Delete` đều `[ValidateAntiForgeryToken]`, gọi `ICategoryApiClient` rồi `RedirectToAction(nameof(Index))` — không trả JSON, vì đây vẫn là MVC form POST thường (qua modal), không phải gọi AJAX. |
| `Controllers/TagController.cs` | `[Authorize(Roles = "Admin")]` | CRUD Tag dạng tương tự `CategoryController` — chỉ Admin được vào (Staff chỉ được "get-or-create" tag nhanh lúc viết tin, qua API riêng ở mục 2.3.6, không qua trang quản lý Tag này). |
| `Controllers/ReportController.cs` | `[Authorize(Roles = "Admin")]` | `Index(startDate, endDate)`: mặc định lấy báo cáo 30 ngày gần nhất nếu không truyền tham số (`DateTime.Today.AddDays(-30)` → `DateTime.Today`); cộng thêm `.AddDays(1).AddTicks(-1)` vào `endDate` để **bao trọn hết ngày cuối** (nếu không, lọc theo `<= endDate` ở BE sẽ bỏ sót các tin tạo trong chính ngày `endDate` vì `DateTime` đó mặc định là `00:00:00`). |

**Vì sao `CategoryController` cho cả Staff lẫn Admin nhưng `TagController` chỉ cho Admin?** Đúng theo phân quyền đã thống nhất ở đầu dự án: Staff được CRUD Category (vì cần tạo category mới khi viết bài thuộc 1 chuyên mục chưa có), nhưng Tag được xem là dữ liệu "chuẩn hoá toàn hệ thống" nên CRUD đầy đủ chỉ Admin được làm — Staff chỉ được **gắn tag có sẵn hoặc tự thêm nhanh 1 tag mới** ngay lúc viết tin (qua `api/tags/get-or-create`, không qua trang `TagController` này).

---

## 4. Tổng kết — Design Pattern đã áp dụng & lý do

| Pattern | Ở đâu | Vì sao |
|---|---|---|
| **3-layer Architecture** | DataAccess / BusinessLogic / Api (BE) | Tách "biết lưu trữ" – "biết luật" – "biết HTTP" để code dễ test, dễ thay đổi 1 phần mà không ảnh hưởng phần khác. |
| **Repository Pattern** | `IGenericRepository`, `ICategoryRepository`,... | Controller/Service không viết LINQ trực tiếp lên `DbContext` — tuân thủ đúng yêu cầu đề: *"không kết nối DB trực tiếp từ Controller"*. |
| **Unit of Work** | `IUnitOfWork` | Đảm bảo nhiều thay đổi trên nhiều repository trong 1 request được lưu (`SaveChanges`) **cùng 1 lần**, tránh nửa-lưu-nửa-không. |
| **Singleton** | `ICategoryTreeCache` | Dữ liệu cây Category đọc rất nhiều, đổi rất ít → cache 1 lần dùng chung toàn app, chỉ build lại khi có CRUD Category. |
| **DTO (Data Transfer Object)** | `Dtos/*.cs` (cả BE & FE) | Không trả thẳng Entity (EF Core) qua API — tránh lộ field nội bộ, tránh vòng lặp tham chiếu khi serialize JSON (`NewsArticle.Tags[].NewsArticles[]...`). |
| **Middleware (Chain of Responsibility)** | `ExceptionHandlingMiddleware` | Xử lý lỗi nghiệp vụ tập trung 1 chỗ, Controller không cần try/catch lặp lại. |
| **Delegating Handler** | `IdentityForwardingHandler` (FE) | Tự động đính danh tính (header `X-Account-*`, trước đây là JWT) vào mọi request HttpClient mà không cần lặp code ở từng API Client. |
| **View Component** | `CategoryMenuViewComponent` (FE) | UI cần tự fetch dữ liệu riêng (cây category), không chỉ render model có sẵn như Partial View thường. |
| **Exception Filter** | `ApiExceptionFilter` (FE) | Bắt `ApiException` (403 sai quyền, 404/409 lỗi nghiệp vụ,...) tập trung 1 nơi cho mọi Controller, tránh app crash (xem mục 3.6). |
| **Authentication Handler** | `HeaderIdentityAuthenticationHandler` (BE) | Thay cho `JwtBearerHandler` đã gỡ — dựng `ClaimsPrincipal` từ header thường do FE gửi, đơn giản hơn JWT nhưng **không có chữ ký** (chỉ an toàn vì FE là caller duy nhất, xem mục 2.3.1). |

---

## 5. Cách chạy lại dự án từ đầu

```bash
# 1. Tạo & seed database (chạy 1 lần)
sqlcmd -S "(localdb)\MSSQLLocalDB" -i "FUNewsManagement.sql"
sqlcmd -S "(localdb)\MSSQLLocalDB" -i "FUNewsManagement_Seed_App.sql"

# 2. Chạy BE (terminal 1)
cd BE/src/FUNewsManagement.Api
dotnet run --urls http://localhost:5224

# 3. Chạy FE (terminal 2)
cd FE/src/FUNewsManagement.Web
dotnet run --urls http://localhost:5100
```

Tài khoản test có sẵn (mật khẩu đã hash, nhập đúng mật khẩu gốc lúc login):
| Email | Mật khẩu | Vai trò |
|---|---|---|
| `admin@FUNewsManagementSystem.org` | `@@abc123@@` | Admin (từ appsettings) |
| `IsabellaDavid@FUNewsManagement.org` | `@1` | Staff |
| `EmmaWilliam@FUNewsManagement.org` | `@1` | Staff (role Lecturer trong DB, vẫn map ra "Staff") |

Trước khi demo đăng nhập Google thật, cần điền **Google OAuth Client ID** thật vào:
- `BE/src/FUNewsManagement.Api/appsettings.json` → `Google:ClientId`
- `FE/src/FUNewsManagement.Web/appsettings.json` → `Google:ClientId`
