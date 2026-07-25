using Q2;
using Q2.Services;

var builder = WebApplication.CreateBuilder(args);

//Initialize UrlUtilities with configuration
//DO NOT change this code
Utilities.Initialize(builder.Configuration);
//End

builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient<ApiClient>();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();

var app = builder.Build();

app.UseSession();
app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Equipments}/{action=Index}/{id?}");

app.Run();
