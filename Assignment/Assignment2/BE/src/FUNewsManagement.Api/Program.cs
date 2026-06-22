using FUNewsManagement.Api.Middleware;
using FUNewsManagement.Api.Security;
using FUNewsManagement.BusinessLogic;
using FUNewsManagement.BusinessLogic.Dtos;
using FUNewsManagement.DataAccess;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.OData;
using Microsoft.OData.ModelBuilder;
using Microsoft.OData.Edm;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

const string CorsPolicy = "FrontEnd";

builder.Services.AddDataAccess(builder.Configuration.GetConnectionString("Default")!);
builder.Services.AddBusinessLogic(builder.Configuration);

builder.Services.AddControllers().AddOData(options =>
    options.Select().Filter().OrderBy().Expand().Count().SetMaxTop(200)
        .AddRouteComponents("odata", GetEdmModel()));

builder.Services.AddCors(options =>
{
    var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();
    options.AddPolicy(CorsPolicy, policy =>
        policy.WithOrigins(allowedOrigins).AllowAnyHeader().AllowAnyMethod());
});

// No JWT for now: the FE forwards the caller's identity as plain headers after its own cookie
// login, and this scheme just trusts them. See HeaderIdentityAuthenticationHandler for details
// and the security caveat (only safe because the FE is the sole caller).
builder.Services.AddAuthentication(HeaderIdentityAuthenticationHandler.SchemeName)
    .AddScheme<AuthenticationSchemeOptions, HeaderIdentityAuthenticationHandler>(HeaderIdentityAuthenticationHandler.SchemeName, null);
builder.Services.AddAuthorization();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "FUNewsManagement API", Version = "v1" });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseCors(CorsPolicy);

app.UseAuthentication();
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
