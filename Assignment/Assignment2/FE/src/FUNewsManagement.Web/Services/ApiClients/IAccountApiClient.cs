using FUNewsManagement.Web.Models.Dtos;

namespace FUNewsManagement.Web.Services.ApiClients;

public interface IAccountApiClient
{
    Task<List<AccountDto>> GetAllAsync();

    Task<AccountDto?> GetByIdAsync(short id);

    Task<AccountDto?> CreateAsync(AccountUpsertDto dto);

    Task UpdateAsync(short id, AccountUpsertDto dto);

    Task DeleteAsync(short id);
}
