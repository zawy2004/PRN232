using FUCourseManagementSystem.WebClient.Models;
using FUCourseManagementSystem.WebClient.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json;

namespace FUCourseManagementSystem.WebClient.Controllers;

[Authorize]
public class EnrollmentsController : Controller
{
    private readonly ApiService _api;

    public EnrollmentsController(ApiService api) => _api = api;

    [Authorize(Roles = "Admin,AcademicStaff,Lecturer")]
    public async Task<IActionResult> Index(string? search)
    {
        var result = await _api.GetAsync<ODataResult<EnrollmentViewModel>>("api/enrollments");
        return View(result?.Value ?? new List<EnrollmentViewModel>());
    }

    [Authorize(Roles = "Student")]
    public async Task<IActionResult> MyEnrollments()
    {
        var result = await _api.GetAsync<ODataResult<EnrollmentViewModel>>("api/enrollments");
        return View(result?.Value ?? new List<EnrollmentViewModel>());
    }

    [HttpPost, Authorize(Roles = "Student")]
    public async Task<IActionResult> Enroll(int classId)
    {
        var accountIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(accountIdClaim, out var accountId) || accountId <= 0)
        {
            TempData["Error"] = "Student account is not available.";
            return RedirectToAction("Index", "ClassSections");
        }

        var studentResult = await _api.GetAsync<ODataResult<StudentViewModel>>(
            $"api/students?$filter=AccountID eq {accountId}");
        var studentId = studentResult?.Value?.FirstOrDefault()?.StudentID;
        if (studentId == null)
        {
            TempData["Error"] = "Student profile not found.";
            return RedirectToAction("Index", "ClassSections");
        }

        var response = await _api.PostAsync("api/enrollments/enroll", new { studentId, classId });
        if (!response.IsSuccessStatusCode)
        {
            var err = await response.Content.ReadAsStringAsync();
            var msg = JsonSerializer.Deserialize<JsonElement>(err).GetProperty("message").GetString();
            TempData["Error"] = msg;
        }
        else
        {
            TempData["Success"] = "Enrolled successfully.";
        }
        return RedirectToAction("Index", "ClassSections");
    }

    [HttpPost, Authorize(Roles = "Student")]
    public async Task<IActionResult> Cancel(int id)
    {
        var accountIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(accountIdClaim, out var accountId) || accountId <= 0)
        {
            TempData["Error"] = "Student account is not available.";
            return RedirectToAction(nameof(MyEnrollments));
        }

        var response = await _api.PutAsync<object>($"api/enrollments/{id}/cancel", new { AccountId = accountId });
        if (!response.IsSuccessStatusCode)
            TempData["Error"] = "Cannot cancel this enrollment.";
        return RedirectToAction(nameof(MyEnrollments));
    }

    [HttpPost, Authorize(Roles = "AcademicStaff,Admin")]
    public async Task<IActionResult> UpdateStatus(int id, int status, string? note)
    {
        var response = await _api.PutAsync($"api/enrollments/{id}/status", new { Status = status, Note = note });
        if (!response.IsSuccessStatusCode)
            TempData["Error"] = "Failed to update enrollment status.";
        return RedirectToAction(nameof(Index));
    }
}
