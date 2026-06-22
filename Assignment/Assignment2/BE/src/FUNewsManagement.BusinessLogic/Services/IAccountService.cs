using FUNewsManagement.BusinessLogic.Dtos;

namespace FUNewsManagement.BusinessLogic.Services;

public interface IAccountService
{
    Task<List<AccountDto>> GetAllAsync();

    Task<AccountDto?> GetByIdAsync(short id);

    Task<AccountDto> CreateAsync(AccountUpsertDto dto);

    Task UpdateAsync(short id, AccountUpsertDto dto);

    Task DeleteAsync(short id);
}
