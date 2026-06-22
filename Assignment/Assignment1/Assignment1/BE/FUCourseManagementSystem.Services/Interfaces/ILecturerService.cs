using FUCourseManagementSystem.BusinessObjects;

namespace FUCourseManagementSystem.Services.Interfaces;

public interface ILecturerService
{
    IQueryable<Lecturer> GetAll();
    Task<Lecturer?> GetByIdAsync(int id);
    Task<Lecturer?> GetByAccountIdAsync(int accountId);
    Task CreateAsync(Lecturer lecturer);
    Task UpdateAsync(Lecturer lecturer);
    Task DeleteAsync(int id);
}
