using FUCourseManagementSystem.BusinessObjects;
using FUCourseManagementSystem.DataAccess;
using FUCourseManagementSystem.DataAccess.DAOs;
using FUCourseManagementSystem.Repositories.Interfaces;

namespace FUCourseManagementSystem.Repositories.Implementations;

public class LecturerRepository : ILecturerRepository
{
    private readonly LecturerDAO _dao;

    public LecturerRepository(AppDbContext context)
        => _dao = new LecturerDAO(context);

    public IQueryable<Lecturer> GetAll() => _dao.GetAll();
    public Task<Lecturer?> GetByIdAsync(int id) => _dao.GetByIdAsync(id);
    public Task<Lecturer?> GetByAccountIdAsync(int accountId) => _dao.GetByAccountIdAsync(accountId);
    public Task AddAsync(Lecturer lecturer) => _dao.AddAsync(lecturer);
    public Task UpdateAsync(Lecturer lecturer) => _dao.UpdateAsync(lecturer);
    public Task DeleteAsync(Lecturer lecturer) => _dao.DeleteAsync(lecturer);
}
