using FUNewsManagementSystem.DataAccess;
using FUNewsManagementSystem.DataAccess.Entities;
using FUNewsManagementSystem.Repositories.Implementations;
using FUNewsManagementSystem.Repositories.Interfaces;
using FUNewsManagementSystem.WebApi.Seeding;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.OData.ModelBuilder;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException("Missing DefaultConnection in appsettings.json.");
}

builder.Services.AddDbContext<FunewsDbContext>(options =>
    options.UseSqlServer(connectionString, sqlOptions =>
        sqlOptions.MigrationsAssembly("FUNewsManagementSystem.WebApi")));

var odataBuilder = new ODataConventionModelBuilder();
odataBuilder.EntitySet<SystemAccount>("SystemAccounts");
odataBuilder.EntitySet<Category>("Categories");
odataBuilder.EntitySet<NewsArticle>("NewsArticles");
odataBuilder.EntitySet<Tag>("Tags");

builder.Services.AddControllers()
    .AddOData(options => options
        .Select()
        .Filter()
        .OrderBy()
        .Expand()
        .Count()
        .SetMaxTop(100)
        .AddRouteComponents("odata", odataBuilder.GetEdmModel()));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
});
builder.Services.Configure<AdminAccountOptions>(builder.Configuration.GetSection("AdminAccount"));

var optionsBuilder = new DbContextOptionsBuilder<FunewsDbContext>();
optionsBuilder.UseSqlServer(connectionString, sqlOptions =>
    sqlOptions.MigrationsAssembly("FUNewsManagementSystem.WebApi"));
DbContextFactory.Instance.SetOptions(optionsBuilder.Options);

builder.Services.AddScoped<ISystemAccountRepository, SystemAccountRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<INewsArticleRepository, NewsArticleRepository>();
builder.Services.AddScoped<ITagRepository, TagRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<FunewsDbContext>();
    await dbContext.Database.MigrateAsync();
    var adminOptions = scope.ServiceProvider.GetRequiredService<Microsoft.Extensions.Options.IOptions<AdminAccountOptions>>().Value;
    await DbSeeder.SeedAdminAsync(dbContext, adminOptions);
    await DbSeeder.SeedMockDataAsync(dbContext, adminOptions);
}

await app.RunAsync();
