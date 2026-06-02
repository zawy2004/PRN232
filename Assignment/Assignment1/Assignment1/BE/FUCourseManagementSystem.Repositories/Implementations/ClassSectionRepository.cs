using FUCourseManagementSystem.BusinessObjects;
using FUCourseManagementSystem.DataAccess;
using FUCourseManagementSystem.DataAccess.DAOs;
using FUCourseManagementSystem.Repositories.Interfaces;

namespace FUCourseManagementSystem.Repositories.Implementations;

public class ClassSectionRepository : IClassSectionRepository
{
    private readonly ClassSectionDAO _dao;

    public ClassSectionRepository(AppDbContext context)
        => _dao = new ClassSectionDAO(context);

    public IQueryable<ClassSection> GetAll() => _dao.GetAll();
    public Task<ClassSection?> GetByIdAsync(int id) => _dao.GetByIdAsync(id);
    public Task<bool> HasEnrollmentsAsync(int classId) => _dao.HasEnrollmentsAsync(classId);
    public Task<int> GetEnrolledCountAsync(int classId) => _dao.GetEnrolledCountAsync(classId);
    public Task AddAsync(ClassSection classSection) => _dao.AddAsync(classSection);
    public Task UpdateAsync(ClassSection classSection) => _dao.UpdateAsync(classSection);
    public Task DeleteAsync(ClassSection classSection) => _dao.DeleteAsync(classSection);
}
