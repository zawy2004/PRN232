using FUCourseManagementSystem.BusinessObjects;
using FUCourseManagementSystem.DataAccess;
using FUCourseManagementSystem.DataAccess.DAOs;
using FUCourseManagementSystem.Repositories.Interfaces;

namespace FUCourseManagementSystem.Repositories.Implementations;

public class DepartmentRepository : IDepartmentRepository
{
    private readonly DepartmentDAO _dao;

    public DepartmentRepository(AppDbContext context)
        => _dao = new DepartmentDAO(context);

    public IQueryable<Department> GetAll() => _dao.GetAll();
    public Task<Department?> GetByIdAsync(int id) => _dao.GetByIdAsync(id);
    public Task<bool> HasCoursesAsync(int departmentId) => _dao.HasCoursesAsync(departmentId);
    public Task AddAsync(Department department) => _dao.AddAsync(department);
    public Task UpdateAsync(Department department) => _dao.UpdateAsync(department);
    public Task DeleteAsync(Department department) => _dao.DeleteAsync(department);
}
