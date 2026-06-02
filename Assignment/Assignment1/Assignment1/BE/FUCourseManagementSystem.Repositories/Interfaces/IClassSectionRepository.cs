using FUCourseManagementSystem.BusinessObjects;

namespace FUCourseManagementSystem.Repositories.Interfaces;

public interface IClassSectionRepository
{
    IQueryable<ClassSection> GetAll();
    Task<ClassSection?> GetByIdAsync(int id);
    Task<bool> HasEnrollmentsAsync(int classId);
    Task<int> GetEnrolledCountAsync(int classId);
    Task AddAsync(ClassSection classSection);
    Task UpdateAsync(ClassSection classSection);
    Task DeleteAsync(ClassSection classSection);
}
