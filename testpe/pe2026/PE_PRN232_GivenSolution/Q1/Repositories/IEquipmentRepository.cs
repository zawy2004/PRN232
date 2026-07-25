using Q1.Models;

namespace Q1.Repositories
{
    public interface IEquipmentRepository
    {
        IQueryable<Equipment> GetQueryable();
        Task<Equipment?> GetByIdAsync(int id);
        Task AddAsync(Equipment e);
        void Update(Equipment e);
        void Remove(Equipment e);
        Task SaveAsync();
    }
}
