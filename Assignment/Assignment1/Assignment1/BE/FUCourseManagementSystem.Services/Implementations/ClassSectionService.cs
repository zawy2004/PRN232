using FUCourseManagementSystem.BusinessObjects;
using FUCourseManagementSystem.Repositories.Interfaces;
using FUCourseManagementSystem.Services.Interfaces;

namespace FUCourseManagementSystem.Services.Implementations;

public class ClassSectionService : IClassSectionService
{
    private readonly IClassSectionRepository _repo;

    public ClassSectionService(IClassSectionRepository repo) => _repo = repo;

    public IQueryable<ClassSection> GetAll() => _repo.GetAll();

    public Task<ClassSection?> GetByIdAsync(int id) => _repo.GetByIdAsync(id);

    public Task<int> GetEnrolledCountAsync(int classId) => _repo.GetEnrolledCountAsync(classId);

    public async Task CreateAsync(ClassSection classSection)
    {
        if (classSection.StartDate >= classSection.EndDate)
            throw new InvalidOperationException("StartDate must be earlier than EndDate.");
        await _repo.AddAsync(classSection);
    }

    public async Task UpdateAsync(ClassSection classSection)
    {
        var existing = await _repo.GetByIdAsync(classSection.ClassID)
            ?? throw new KeyNotFoundException("Class section not found.");
        if (classSection.StartDate >= classSection.EndDate)
            throw new InvalidOperationException("StartDate must be earlier than EndDate.");
        existing.ClassCode = classSection.ClassCode;
        existing.CourseID = classSection.CourseID;
        existing.LecturerID = classSection.LecturerID;
        existing.Semester = classSection.Semester;
        existing.Room = classSection.Room;
        existing.Slot = classSection.Slot;
        existing.StartDate = classSection.StartDate;
        existing.EndDate = classSection.EndDate;
        existing.MaxStudents = classSection.MaxStudents;
        existing.IsActive = classSection.IsActive;
        await _repo.UpdateAsync(existing);
    }

    public async Task DeleteAsync(int id)
    {
        var section = await _repo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("Class section not found.");
        if (await _repo.HasEnrollmentsAsync(id))
            throw new InvalidOperationException("Cannot delete class section with existing enrollments.");
        await _repo.DeleteAsync(section);
    }
}
