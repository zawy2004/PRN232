using Microsoft.EntityFrameworkCore;
using Q1.Models;

namespace Q1.Repositories
{
    public class EquipmentRepository : IEquipmentRepository
    {
        private readonly FPTGearDbContext _db;
        public EquipmentRepository(FPTGearDbContext db) => _db = db;

        public IQueryable<Equipment> GetQueryable() => _db.Equipments.Include(e => e.User);
        public Task<Equipment?> GetByIdAsync(int id) => _db.Equipments.Include(e => e.User).FirstOrDefaultAsync(e => e.EquipmentId == id);
        public async Task AddAsync(Equipment e) => await _db.Equipments.AddAsync(e);
        public void Update(Equipment e) => _db.Equipments.Update(e);
        public void Remove(Equipment e) => _db.Equipments.Remove(e);
        public Task SaveAsync() => _db.SaveChangesAsync();
    }
}
