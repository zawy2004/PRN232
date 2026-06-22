using FUCourseManagementSystem.BusinessObjects;

namespace FUCourseManagementSystem.Repositories.Interfaces;

public interface ILecturerRepository
{
    IQueryable<Lecturer> GetAll();
    Task<Lecturer?> GetByIdAsync(int id);
    Task<Lecturer?> GetByAccountIdAsync(int accountId);
    Task AddAsync(Lecturer lecturer);
    Task UpdateAsync(Lecturer lecturer);
    Task DeleteAsync(Lecturer lecturer);
}
