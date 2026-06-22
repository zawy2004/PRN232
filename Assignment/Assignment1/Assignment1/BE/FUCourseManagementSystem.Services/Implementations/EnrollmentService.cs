using FUCourseManagementSystem.BusinessObjects;
using FUCourseManagementSystem.Repositories.Interfaces;
using FUCourseManagementSystem.Services.Interfaces;

namespace FUCourseManagementSystem.Services.Implementations;

public class EnrollmentService : IEnrollmentService
{
    private readonly IEnrollmentRepository _enrollRepo;
    private readonly IClassSectionRepository _classRepo;
    private readonly IStudentRepository _studentRepo;

    public EnrollmentService(
        IEnrollmentRepository enrollRepo,
        IClassSectionRepository classRepo,
        IStudentRepository studentRepo)
    {
        _enrollRepo = enrollRepo;
        _classRepo = classRepo;
        _studentRepo = studentRepo;
    }

    public IQueryable<Enrollment> GetAll() => _enrollRepo.GetAll();

    public Task<Enrollment?> GetByIdAsync(int id) => _enrollRepo.GetByIdAsync(id);

    public async Task EnrollAsync(int studentId, int classId)
    {
        var student = await _studentRepo.GetByIdAsync(studentId);
        if (student == null || !student.IsActive)
            throw new KeyNotFoundException("Student not found.");

        var classSection = await _classRepo.GetByIdAsync(classId)
            ?? throw new KeyNotFoundException("Class section not found.");

        if (!classSection.IsActive || classSection.Course == null || !classSection.Course.IsActive)
            throw new InvalidOperationException("Cannot enroll in an inactive course or class section.");

        var existing = await _enrollRepo.GetByStudentAndClassAsync(studentId, classId);
        if (existing != null)
        {
            if (existing.EnrollmentStatus == EnrollmentStatus.Cancelled)
            {
                // Reactivate cancelled enrollment instead of creating a duplicate (preserve history)
                existing.EnrollmentStatus = EnrollmentStatus.Pending;
                existing.EnrolledDate = DateTime.UtcNow;
                await _enrollRepo.UpdateAsync(existing);
                return;
            }
            throw new InvalidOperationException("Student is already enrolled in this class section.");
        }

        var enrolled = await _classRepo.GetEnrolledCountAsync(classId);
        if (enrolled >= classSection.MaxStudents)
            throw new InvalidOperationException("Class section has reached the maximum number of students.");

        await _enrollRepo.AddAsync(new Enrollment
        {
            StudentID = studentId,
            ClassID = classId,
            EnrolledDate = DateTime.UtcNow,
            EnrollmentStatus = EnrollmentStatus.Pending
        });
    }

    public Task<Enrollment?> GetByStudentAndClassAsync(int studentId, int classId)
        => _enrollRepo.GetByStudentAndClassAsync(studentId, classId);

    public async Task UpdateStatusAsync(int enrollmentId, EnrollmentStatus status, string? note = null)
    {
        var enrollment = await _enrollRepo.GetByIdAsync(enrollmentId)
            ?? throw new KeyNotFoundException("Enrollment not found.");

        if (status == EnrollmentStatus.Rejected && string.IsNullOrWhiteSpace(note))
            throw new InvalidOperationException("Rejection reason is required when rejecting an enrollment.");

        enrollment.EnrollmentStatus = status;
        if (note != null) enrollment.Note = note;
        await _enrollRepo.UpdateAsync(enrollment);
    }

    public async Task CancelAsync(int enrollmentId, int studentId)
    {
        var enrollment = await _enrollRepo.GetByIdAsync(enrollmentId)
            ?? throw new KeyNotFoundException("Enrollment not found.");

        if (enrollment.StudentID != studentId)
            throw new UnauthorizedAccessException("Cannot cancel another student's enrollment.");

        if (enrollment.EnrollmentStatus != EnrollmentStatus.Pending)
            throw new InvalidOperationException("Only pending enrollments can be cancelled.");

        enrollment.EnrollmentStatus = EnrollmentStatus.Cancelled;
        await _enrollRepo.UpdateAsync(enrollment);
    }

    public async Task DeleteAsync(int id)
    {
        var enrollment = await _enrollRepo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("Enrollment not found.");
        await _enrollRepo.DeleteAsync(enrollment);
    }
}
