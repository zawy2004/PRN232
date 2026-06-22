using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using ODataASPNETCoreDemo.Data;
using ODataASPNETCoreDemo.Data.Entities;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<MyWorldDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("MyWorldDbConnection"));
});

builder.Services.AddControllers().AddOData(option => option.Select().Filter()
    .Count().OrderBy().Expand().SetMaxTop(100)
    .AddRouteComponents("odata", GetEdmModel()));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "ODataASPNETCoreDemo", Version = "v1" });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();

static IEdmModel GetEdmModel()
{
    ODataConventionModelBuilder modelBuilder = new ODataConventionModelBuilder();
    modelBuilder.EntitySet<Gadgets>("GadgetsOdata");
    return modelBuilder.GetEdmModel();
}
