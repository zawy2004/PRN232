using FUNewsManagement.Web.Common;
using FUNewsManagement.Web.Services.ApiClients;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews(options => options.Filters.Add<ApiExceptionFilter>());
builder.Services.AddHttpContextAccessor();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromHours(2);
    });
builder.Services.AddAuthorization();

builder.Services.AddTransient<IdentityForwardingHandler>();

var apiBaseUrl = builder.Configuration["Api:BaseUrl"]!;

builder.Services.AddHttpClient<IAuthApiClient, AuthApiClient>(c => c.BaseAddress = new Uri(apiBaseUrl));
builder.Services.AddHttpClient<ICategoryApiClient, CategoryApiClient>(c => c.BaseAddress = new Uri(apiBaseUrl))
    .AddHttpMessageHandler<IdentityForwardingHandler>();
builder.Services.AddHttpClient<ITagApiClient, TagApiClient>(c => c.BaseAddress = new Uri(apiBaseUrl))
    .AddHttpMessageHandler<IdentityForwardingHandler>();
builder.Services.AddHttpClient<INewsApiClient, NewsApiClient>(c => c.BaseAddress = new Uri(apiBaseUrl))
    .AddHttpMessageHandler<IdentityForwardingHandler>();
builder.Services.AddHttpClient<IAccountApiClient, AccountApiClient>(c => c.BaseAddress = new Uri(apiBaseUrl))
    .AddHttpMessageHandler<IdentityForwardingHandler>();
builder.Services.AddHttpClient<IReportApiClient, ReportApiClient>(c => c.BaseAddress = new Uri(apiBaseUrl))
    .AddHttpMessageHandler<IdentityForwardingHandler>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
