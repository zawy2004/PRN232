# FUNewsManagementSystem Web API - Tong hop trien khai

## Tong quan
Tai lieu nay tong hop toan bo phan Web API da trien khai, cac thanh phan chinh, va cach no map voi yeu cau trong file bai tap.

## Cau truc solution (Kien truc 3 lop)
- Lop Web API: FUNewsManagementSystem.WebApi
- Lop Repository: FUNewsManagementSystem.Repositories
- Lop Data Access: FUNewsManagementSystem.DataAccess

## Cau hinh chung
- appsettings.json:
  - Connection string va tai khoan admin mac dinh.
  - Admin duoc seed tu dong khi khoi dong ung dung.

## Mo hinh du lieu (Entities)
Da trien khai cac entity theo thiet ke CSDL:
- SystemAccount: thong tin tai khoan, vai tro, lien ket bai viet tao/cap nhat.
- Category: thong tin danh muc, trang thai, quan he cha-con.
- NewsArticle: noi dung bai viet, trang thai, danh muc, nguoi tao, nguoi cap nhat, the (tags).
- Tag: thong tin the.
- NewsTag: bang noi giua NewsArticle va Tag (many-to-many).

File chinh:
- FUNewsManagementSystem.DataAccess/Entities/SystemAccount.cs
- FUNewsManagementSystem.DataAccess/Entities/Category.cs
- FUNewsManagementSystem.DataAccess/Entities/NewsArticle.cs
- FUNewsManagementSystem.DataAccess/Entities/Tag.cs
- FUNewsManagementSystem.DataAccess/Entities/NewsTag.cs

## DbContext va Mapping
- FunewsDbContext khai bao DbSet va cau hinh quan he.
- NewsArticle - Tag la many-to-many qua NewsTag.
- Cac rang buoc xoa (DeleteBehavior) dam bao dung yeu cau bai.

File chinh:
- FUNewsManagementSystem.DataAccess/FunewsDbContext.cs

## DAO + Repository (Singleton + Repository)
- DAO su dung Singleton, thao tac voi EF Core.
- Repository bao boc DAO, xuat interface cho Controller.

DAO:
- FUNewsManagementSystem.DataAccess/Daos/SystemAccountDao.cs
- FUNewsManagementSystem.DataAccess/Daos/CategoryDao.cs
- FUNewsManagementSystem.DataAccess/Daos/NewsArticleDao.cs
- FUNewsManagementSystem.DataAccess/Daos/TagDao.cs

Repository:
- FUNewsManagementSystem.Repositories/Interfaces/ISystemAccountRepository.cs
- FUNewsManagementSystem.Repositories/Interfaces/ICategoryRepository.cs
- FUNewsManagementSystem.Repositories/Interfaces/INewsArticleRepository.cs
- FUNewsManagementSystem.Repositories/Interfaces/ITagRepository.cs
- FUNewsManagementSystem.Repositories/Implementations/SystemAccountRepository.cs
- FUNewsManagementSystem.Repositories/Implementations/CategoryRepository.cs
- FUNewsManagementSystem.Repositories/Implementations/NewsArticleRepository.cs
- FUNewsManagementSystem.Repositories/Implementations/TagRepository.cs

## Web API (OData + REST)
- CRUD su dung OData cho Account, Category, News, Tag.
- Co them REST endpoint cho public news va lich su theo nguoi tao.

OData controllers:
- FUNewsManagementSystem.WebApi/Controllers/OData/SystemAccountsController.cs
- FUNewsManagementSystem.WebApi/Controllers/OData/CategoriesController.cs
- FUNewsManagementSystem.WebApi/Controllers/OData/NewsArticlesController.cs
- FUNewsManagementSystem.WebApi/Controllers/OData/TagsController.cs

Auth controller:
- FUNewsManagementSystem.WebApi/Controllers/AuthController.cs

DTO:
- FUNewsManagementSystem.WebApi/Models/LoginRequest.cs
- FUNewsManagementSystem.WebApi/Models/NewsArticleCreateDto.cs
- FUNewsManagementSystem.WebApi/Models/NewsArticleUpdateDto.cs

## Cau hinh khoi dong (Program.cs)
- Dang ky OData model cho SystemAccounts, Categories, NewsArticles, Tags.
- Cau hinh EF Core voi SQL Server va migrations assembly.
- Swagger duoc bat trong Development.
- Tu dong migrate va seed du lieu khi khoi dong.

File chinh:
- FUNewsManagementSystem.WebApi/Program.cs

## Seed du lieu
- Admin duoc tao tu appsettings.json neu chua co.
- Mock data cho staff/lecturer, category, tag, news (co tag).

File chinh:
- FUNewsManagementSystem.WebApi/Seeding/AdminAccountOptions.cs
- FUNewsManagementSystem.WebApi/Seeding/DbSeeder.cs

## Tu dong mo Swagger
- Visual Studio launch profile bat tu dong mo Swagger.

File chinh:
- FUNewsManagementSystem.WebApi/Properties/launchSettings.json

## Endpoint chinh
Base URL (Development):
- https://localhost:7234
- http://localhost:5284

Swagger:
- https://localhost:7234/swagger

OData:
- GET /odata/SystemAccounts
- GET /odata/Categories
- GET /odata/NewsArticles
- GET /odata/Tags

Public news:
- GET /api/news/active

Auth:
- POST /api/Auth/login

History theo nguoi tao:
- GET /api/news/creator/{accountId}

## Cach chay
1) Chay migrations (neu can)
- dotnet ef database update --project .\FUNewsManagementSystem.WebApi\FUNewsManagementSystem.WebApi.csproj --startup-project .\FUNewsManagementSystem.WebApi\FUNewsManagementSystem.WebApi.csproj --context FunewsDbContext

2) Chay API
- dotnet run --project .\FUNewsManagementSystem.WebApi\FUNewsManagementSystem.WebApi.csproj

3) Mo Swagger
- https://localhost:7234/swagger

## Doi chieu yeu cau bai tap
- CRUD + Search: dung OData ($filter, $orderby, $top, $skip, $expand).
- DAO + Repository: dam bao controller khong truy cap DbContext truc tiep.
- Admin mac dinh: seed tu appsettings.json.
- Rang buoc xoa: account va category chi xoa khi khong bi tham chieu.
- Xem news khong can auth: GET /api/news/active.

## Cac phan chua co
- Client app (web/desktop) chua trien khai.
- Role-based authorization o API chua bat (login chi tra ve thong tin tai khoan).
