using FUCourseManagementSystem.BusinessObjects;

namespace FUCourseManagementSystem.Services.Interfaces;

public interface IClassSectionService
{
    IQueryable<ClassSection> GetAll();
    Task<ClassSection?> GetByIdAsync(int id);
    Task<int> GetEnrolledCountAsync(int classId);
    Task CreateAsync(ClassSection classSection);
    Task UpdateAsync(ClassSection classSection);
    Task DeleteAsync(int id);
}
