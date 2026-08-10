using Microsoft.EntityFrameworkCore;
using Q1.Models;
using System;

var builder = WebApplication.CreateBuilder(args);

// Database connection string is adjustable through appsettings.json ("MyCnn").
builder.Services.AddDbContext<Prn232PeSu2611Context>(options =>
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
