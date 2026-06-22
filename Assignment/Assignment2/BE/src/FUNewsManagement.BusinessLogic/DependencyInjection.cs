using FUNewsManagement.BusinessLogic.Caching;
using FUNewsManagement.BusinessLogic.Options;
using FUNewsManagement.BusinessLogic.Security;
using FUNewsManagement.BusinessLogic.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FUNewsManagement.BusinessLogic;

public static class DependencyInjection
{
    public static IServiceCollection AddBusinessLogic(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<AdminAccountOptions>(configuration.GetSection("AdminAccount"));
        services.Configure<GoogleAuthOptions>(configuration.GetSection("Google"));

        services.AddSingleton<ICategoryTreeCache, CategoryTreeCache>();

        services.AddSingleton<IGoogleTokenValidator, GoogleTokenValidator>();

        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<ITagService, TagService>();
        services.AddScoped<IAccountService, AccountService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<INewsService, NewsService>();

        return services;
    }
}
