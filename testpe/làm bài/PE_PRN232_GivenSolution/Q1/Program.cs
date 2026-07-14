using Microsoft.EntityFrameworkCore;
using Q1.Data;

var builder = WebApplication.CreateBuilder(args);

// Database connection string is adjustable through appsettings.json ("MyCnn").
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MyCnn")));

// Support response formats in both JSON and XML using media formatters.
builder.Services.AddControllers()
    .AddXmlSerializerFormatters();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
