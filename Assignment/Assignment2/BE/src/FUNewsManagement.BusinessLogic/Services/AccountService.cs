using FUNewsManagement.BusinessLogic.Common;
using FUNewsManagement.BusinessLogic.Dtos;
using FUNewsManagement.BusinessLogic.Exceptions;
using FUNewsManagement.BusinessLogic.Options;
using FUNewsManagement.BusinessLogic.Security;
using FUNewsManagement.DataAccess.Entities;
using FUNewsManagement.DataAccess.UnitOfWork;
using Microsoft.Extensions.Options;

namespace FUNewsManagement.BusinessLogic.Services;

public class AccountService : IAccountService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly AdminAccountOptions _adminOptions;

    public AccountService(IUnitOfWork unitOfWork, IOptions<AdminAccountOptions> adminOptions)
    {
        _unitOfWork = unitOfWork;
        _adminOptions = adminOptions.Value;
    }

    public async Task<List<AccountDto>> GetAllAsync()
    {
        var accounts = await _unitOfWork.Accounts.GetAllAsync();
        return accounts.Where(a => a.AccountId != _adminOptions.AccountId).Select(MapEntity).ToList();
    }

    public async Task<AccountDto?> GetByIdAsync(short id)
    {
        var account = await _unitOfWork.Accounts.GetByIdAsync(id);
        return account is null ? null : MapEntity(account);
    }

    public async Task<AccountDto> CreateAsync(AccountUpsertDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Password))
            throw new BusinessRuleException("Password is required when creating an account.");

        if (string.Equals(dto.AccountEmail, _adminOptions.Email, StringComparison.OrdinalIgnoreCase))
            throw new BusinessRuleException("This email is reserved for the system Admin account.");

        if (await _unitOfWork.Accounts.GetByEmailAsync(dto.AccountEmail) is not null)
            throw new BusinessRuleException("An account with this email already exists.");

        var entity = new SystemAccount
        {
            AccountId = await _unitOfWork.Accounts.GetNextIdAsync(),
            AccountName = dto.AccountName,
            AccountEmail = dto.AccountEmail,
            AccountRole = dto.AccountRole,
            AccountPassword = PasswordHasher.Hash(dto.Password),
        };

        await _unitOfWork.Accounts.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();

        return MapEntity(entity);
    }

    public async Task UpdateAsync(short id, AccountUpsertDto dto)
    {
        if (id == _adminOptions.AccountId)
            throw new BusinessRuleException("The system Admin account cannot be modified here.");

        var entity = await _unitOfWork.Accounts.GetByIdAsync(id)
            ?? throw new EntityNotFoundException($"Account {id} not found.");

        entity.AccountName = dto.AccountName;
        entity.AccountEmail = dto.AccountEmail;
        entity.AccountRole = dto.AccountRole;
        if (!string.IsNullOrWhiteSpace(dto.Password))
            entity.AccountPassword = PasswordHasher.Hash(dto.Password);

        _unitOfWork.Accounts.Update(entity);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteAsync(short id)
    {
        if (id == _adminOptions.AccountId)
            throw new BusinessRuleException("The system Admin account cannot be deleted.");

        var entity = await _unitOfWork.Accounts.GetByIdAsync(id)
            ?? throw new EntityNotFoundException($"Account {id} not found.");

        if (await _unitOfWork.Accounts.HasCreatedNewsArticlesAsync(id))
            throw new BusinessRuleException("Cannot delete an account that has created news articles.");

        _unitOfWork.Accounts.Remove(entity);
        await _unitOfWork.SaveChangesAsync();
    }

    private static AccountDto MapEntity(SystemAccount a) => new()
    {
        AccountId = a.AccountId,
        AccountName = a.AccountName ?? string.Empty,
        AccountEmail = a.AccountEmail ?? string.Empty,
        AccountRole = a.AccountRole ?? 0,
        RoleName = RoleNames.FromAccountRole(a.AccountRole),
    };
}
