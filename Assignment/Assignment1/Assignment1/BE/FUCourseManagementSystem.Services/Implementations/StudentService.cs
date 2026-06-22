using FUCourseManagementSystem.BusinessObjects;
using FUCourseManagementSystem.Repositories.Interfaces;
using FUCourseManagementSystem.Services.Interfaces;

namespace FUCourseManagementSystem.Services.Implementations;

public class StudentService : IStudentService
{
    private readonly IStudentRepository _repo;

    public StudentService(IStudentRepository repo) => _repo = repo;

    public IQueryable<Student> GetAll() => _repo.GetAll();

    public Task<Student?> GetByIdAsync(int id) => _repo.GetByIdAsync(id);

    public Task<Student?> GetByAccountIdAsync(int accountId) => _repo.GetByAccountIdAsync(accountId);

    public Task CreateAsync(Student student) => _repo.AddAsync(student);

    public async Task UpdateAsync(Student student)
    {
        var existing = await _repo.GetByIdAsync(student.StudentID)
            ?? throw new KeyNotFoundException("Student not found.");
        existing.StudentCode = student.StudentCode;
        existing.StudentName = student.StudentName;
        existing.StudentEmail = student.StudentEmail;
        existing.Major = student.Major;
        existing.IsActive = student.IsActive;
        await _repo.UpdateAsync(existing);
    }

    public async Task DeleteAsync(int id)
    {
        var student = await _repo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("Student not found.");
        await _repo.DeleteAsync(student);
    }
}
