using FUCourseManagementSystem.BusinessObjects;
using FUCourseManagementSystem.Repositories.Interfaces;
using FUCourseManagementSystem.Services.Interfaces;

namespace FUCourseManagementSystem.Services.Implementations;

public class DepartmentService : IDepartmentService
{
    private readonly IDepartmentRepository _repo;

    public DepartmentService(IDepartmentRepository repo) => _repo = repo;

    public IQueryable<Department> GetAll() => _repo.GetAll();

    public Task<Department?> GetByIdAsync(int id) => _repo.GetByIdAsync(id);

    public Task CreateAsync(Department department) => _repo.AddAsync(department);

    public async Task UpdateAsync(Department department)
    {
        var existing = await _repo.GetByIdAsync(department.DepartmentID)
            ?? throw new KeyNotFoundException("Department not found.");
        existing.DepartmentName = department.DepartmentName;
        existing.DepartmentCode = department.DepartmentCode;
        existing.Description = department.Description;
        existing.IsActive = department.IsActive;
        await _repo.UpdateAsync(existing);
    }

    public async Task DeleteAsync(int id)
    {
        var dept = await _repo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("Department not found.");
        if (await _repo.HasCoursesAsync(id))
            throw new InvalidOperationException("Cannot delete department that has courses.");
        await _repo.DeleteAsync(dept);
    }
}
