using FUCourseManagementSystem.BusinessObjects;
using FUCourseManagementSystem.Repositories.Interfaces;
using FUCourseManagementSystem.Services.Interfaces;

namespace FUCourseManagementSystem.Services.Implementations;

public class LecturerService : ILecturerService
{
    private readonly ILecturerRepository _repo;

    public LecturerService(ILecturerRepository repo) => _repo = repo;

    public IQueryable<Lecturer> GetAll() => _repo.GetAll();

    public Task<Lecturer?> GetByIdAsync(int id) => _repo.GetByIdAsync(id);

    public Task<Lecturer?> GetByAccountIdAsync(int accountId) => _repo.GetByAccountIdAsync(accountId);

    public Task CreateAsync(Lecturer lecturer) => _repo.AddAsync(lecturer);

    public async Task UpdateAsync(Lecturer lecturer)
    {
        var existing = await _repo.GetByIdAsync(lecturer.LecturerID)
            ?? throw new KeyNotFoundException("Lecturer not found.");
        existing.LecturerName = lecturer.LecturerName;
        existing.LecturerEmail = lecturer.LecturerEmail;
        existing.PhoneNumber = lecturer.PhoneNumber;
        existing.DepartmentID = lecturer.DepartmentID;
        existing.IsActive = lecturer.IsActive;
        await _repo.UpdateAsync(existing);
    }

    public async Task DeleteAsync(int id)
    {
        var lecturer = await _repo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("Lecturer not found.");
        await _repo.DeleteAsync(lecturer);
    }
}
