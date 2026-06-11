using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using ODataDemo2.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<BookStoreContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers()
    .AddOData(option => option.Select().Filter()
        .Count().OrderBy().Expand().SetMaxTop(100)
        .AddRouteComponents("odata", GetEdmModel()));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<BookStoreContext>();
    if (!db.Books.Any())
    {
        foreach (var book in DataSource.GetBooks())
        {
            db.Books.Add(book);
        }
        db.SaveChanges();
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

static IEdmModel GetEdmModel()
{
    ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
    builder.EntitySet<Book>("Books");
    builder.EntitySet<Press>("Presses");
    return builder.GetEdmModel();
}
