using FUNewsManagement.Web.Models.Dtos;

namespace FUNewsManagement.Web.Services.ApiClients;

public interface IAuthApiClient
{
    Task<LoginResultDto?> LoginAsync(string email, string password);

    Task<LoginResultDto?> GoogleLoginAsync(string idToken);
}
