using FUNewsManagement.BusinessLogic.Common;
using FUNewsManagement.BusinessLogic.Dtos;
using FUNewsManagement.BusinessLogic.Options;
using FUNewsManagement.BusinessLogic.Security;
using FUNewsManagement.DataAccess.Entities;
using FUNewsManagement.DataAccess.UnitOfWork;
using Microsoft.Extensions.Options;

namespace FUNewsManagement.BusinessLogic.Services;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IGoogleTokenValidator _googleTokenValidator;
    private readonly AdminAccountOptions _adminOptions;

    public AuthService(
        IUnitOfWork unitOfWork,
        IGoogleTokenValidator googleTokenValidator,
        IOptions<AdminAccountOptions> adminOptions)
    {
        _unitOfWork = unitOfWork;
        _googleTokenValidator = googleTokenValidator;
        _adminOptions = adminOptions.Value;
    }

    public async Task<LoginResultDto?> LoginAsync(LoginRequestDto dto)
    {
        if (IsAdminEmail(dto.Email))
        {
            return dto.Password == _adminOptions.Password ? AdminIdentity() : null;
        }

        var account = await _unitOfWork.Accounts.GetByEmailAsync(dto.Email);
        if (account is null || !PasswordHasher.Verify(dto.Password, account.AccountPassword))
            return null;

        return AccountIdentity(account.AccountId, account.AccountEmail ?? string.Empty, account.AccountName ?? string.Empty, account.AccountRole);
    }

    public async Task<LoginResultDto?> GoogleLoginAsync(GoogleLoginRequestDto dto)
    {
        var google = await _googleTokenValidator.ValidateAsync(dto.IdToken);
        if (google is null) return null;

        if (IsAdminEmail(google.Email))
            return AdminIdentity();

        var account = await _unitOfWork.Accounts.GetByEmailAsync(google.Email);
        if (account is null)
        {
            account = new SystemAccount
            {
                AccountId = await _unitOfWork.Accounts.GetNextIdAsync(),
                AccountName = string.IsNullOrWhiteSpace(google.Name) ? google.Email : google.Name,
                AccountEmail = google.Email,
                AccountRole = (int)AccountRole.Staff,
                AccountPassword = null, // tài khoản tạo từ Google, chưa có mật khẩu nội bộ
            };
            await _unitOfWork.Accounts.AddAsync(account);
            await _unitOfWork.SaveChangesAsync();
        }

        return AccountIdentity(account.AccountId, account.AccountEmail ?? string.Empty, account.AccountName ?? string.Empty, account.AccountRole);
    }

    private bool IsAdminEmail(string email) =>
        string.Equals(email, _adminOptions.Email, StringComparison.OrdinalIgnoreCase);

    private LoginResultDto AdminIdentity() => new()
    {
        AccountId = _adminOptions.AccountId,
        AccountName = _adminOptions.AccountName,
        Email = _adminOptions.Email,
        Role = RoleNames.Admin,
    };

    private static LoginResultDto AccountIdentity(short accountId, string email, string name, int? accountRole) => new()
    {
        AccountId = accountId,
        AccountName = name,
        Email = email,
        Role = RoleNames.FromAccountRole(accountRole),
    };
}
