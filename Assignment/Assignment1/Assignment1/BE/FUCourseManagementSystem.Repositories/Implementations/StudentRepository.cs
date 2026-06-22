using FUCourseManagementSystem.BusinessObjects;
using FUCourseManagementSystem.DataAccess;
using FUCourseManagementSystem.DataAccess.DAOs;
using FUCourseManagementSystem.Repositories.Interfaces;

namespace FUCourseManagementSystem.Repositories.Implementations;

public class StudentRepository : IStudentRepository
{
    private readonly StudentDAO _dao;

    public StudentRepository(AppDbContext context)
        => _dao = new StudentDAO(context);

    public IQueryable<Student> GetAll() => _dao.GetAll();
    public Task<Student?> GetByIdAsync(int id) => _dao.GetByIdAsync(id);
    public Task<Student?> GetByAccountIdAsync(int accountId) => _dao.GetByAccountIdAsync(accountId);
    public Task AddAsync(Student student) => _dao.AddAsync(student);
    public Task UpdateAsync(Student student) => _dao.UpdateAsync(student);
    public Task DeleteAsync(Student student) => _dao.DeleteAsync(student);
}
