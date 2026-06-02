using FUCourseManagementSystem.BusinessObjects;
using FUCourseManagementSystem.DataAccess;
using FUCourseManagementSystem.DataAccess.DAOs;
using FUCourseManagementSystem.Repositories.Interfaces;

namespace FUCourseManagementSystem.Repositories.Implementations;

public class EnrollmentRepository : IEnrollmentRepository
{
    private readonly EnrollmentDAO _dao;

    public EnrollmentRepository(AppDbContext context)
        => _dao = new EnrollmentDAO(context);

    public IQueryable<Enrollment> GetAll() => _dao.GetAll();
    public Task<Enrollment?> GetByIdAsync(int id) => _dao.GetByIdAsync(id);
    public Task<bool> ExistsAsync(int studentId, int classId) => _dao.ExistsAsync(studentId, classId);
    public Task<Enrollment?> GetByStudentAndClassAsync(int studentId, int classId) => _dao.GetByStudentAndClassAsync(studentId, classId);
    public Task AddAsync(Enrollment enrollment) => _dao.AddAsync(enrollment);
    public Task UpdateAsync(Enrollment enrollment) => _dao.UpdateAsync(enrollment);
    public Task DeleteAsync(Enrollment enrollment) => _dao.DeleteAsync(enrollment);
}
