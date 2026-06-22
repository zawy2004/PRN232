using FUNewsManagement.BusinessLogic.Dtos;

namespace FUNewsManagement.BusinessLogic.Services;

public interface IAuthService
{
    Task<LoginResultDto?> LoginAsync(LoginRequestDto dto);

    Task<LoginResultDto?> GoogleLoginAsync(GoogleLoginRequestDto dto);
}
