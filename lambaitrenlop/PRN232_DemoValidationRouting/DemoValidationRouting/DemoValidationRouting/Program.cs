using Microsoft.EntityFrameworkCore;
using DemoValidationRouting.Data;
using AutoMapper;
using DemoValidationRouting.Mapping;
using DemoValidationRouting.Repository;
using DemoValidationRouting.Interface;
using DemoValidationRouting.Logger;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("sqlConnection")));
// AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// Repository & logging
builder.Services.AddScoped<IRepositoryManager, RepositoryManager>();
builder.Services.AddSingleton<ILoggerManager, LoggerManager>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Áp dụng migration tự động khi khởi động (giúp demo hoạt động nếu DB chưa có bảng)
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    context.Database.Migrate();
}

// Disable HTTPS redirection to avoid potential mixed-content or redirect issues during local Swagger testing.
// app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
