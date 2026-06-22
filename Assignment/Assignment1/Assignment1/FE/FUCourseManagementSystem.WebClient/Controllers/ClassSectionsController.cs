using FUCourseManagementSystem.WebClient.Models;
using FUCourseManagementSystem.WebClient.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FUCourseManagementSystem.WebClient.Controllers;

public class ClassSectionsController : Controller
{
    private readonly ApiService _api;

    public ClassSectionsController(ApiService api) => _api = api;

    public async Task<IActionResult> Index(string? search, string? semester)
    {
        var url = "api/classsections";
        var filters = new List<string>();
        if (!string.IsNullOrWhiteSpace(search))
            filters.Add($"(contains(ClassCode,'{search}') or contains(CourseName,'{search}'))");
        if (!string.IsNullOrWhiteSpace(semester))
            filters.Add($"Semester eq '{semester}'");
        if (filters.Any())
            url += "?$filter=" + string.Join(" and ", filters);

        var result = await _api.GetAsync<ODataResult<ClassSectionViewModel>>(url);
        ViewBag.Search = search;
        ViewBag.Semester = semester;
        return View(result?.Value ?? new List<ClassSectionViewModel>());
    }

    public async Task<IActionResult> Details(int id)
    {
        var section = await _api.GetAsync<ClassSectionViewModel>($"api/classsections/{id}");
        return section == null ? NotFound() : View(section);
    }

    [Authorize(Roles = "AcademicStaff,Admin")]
    public async Task<IActionResult> Create()
    {
        await LoadDropdowns();
        return View(new ClassSectionViewModel { StartDate = DateTime.Today, EndDate = DateTime.Today.AddMonths(4) });
    }

    [HttpPost, Authorize(Roles = "AcademicStaff,Admin")]
    public async Task<IActionResult> Create(ClassSectionViewModel model)
    {
        if (!ModelState.IsValid) { await LoadDropdowns(); return View(model); }
        var response = await _api.PostAsync("api/classsections", model);
        if (response.IsSuccessStatusCode) return RedirectToAction(nameof(Index));
        ModelState.AddModelError("", "Failed to create class section.");
        await LoadDropdowns();
        return View(model);
    }

    [Authorize(Roles = "AcademicStaff,Admin")]
    public async Task<IActionResult> Edit(int id)
    {
        var section = await _api.GetAsync<ClassSectionViewModel>($"api/classsections/{id}");
        if (section == null) return NotFound();
        await LoadDropdowns();
        return View(section);
    }

    [HttpPost, Authorize(Roles = "AcademicStaff,Admin")]
    public async Task<IActionResult> Edit(int id, ClassSectionViewModel model)
    {
        if (!ModelState.IsValid) { await LoadDropdowns(); return View(model); }
        var response = await _api.PutAsync($"api/classsections/{id}", model);
        if (response.IsSuccessStatusCode) return RedirectToAction(nameof(Index));
        ModelState.AddModelError("", "Failed to update class section.");
        await LoadDropdowns();
        return View(model);
    }

    [HttpPost, Authorize(Roles = "AcademicStaff,Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var response = await _api.DeleteAsync($"api/classsections/{id}");
        if (!response.IsSuccessStatusCode)
            TempData["Error"] = "Cannot delete class section with existing enrollments.";
        return RedirectToAction(nameof(Index));
    }

    private async Task LoadDropdowns()
    {
        var courses = await _api.GetAsync<ODataResult<CourseViewModel>>("api/courses");
        var lecturers = await _api.GetAsync<ODataResult<LecturerViewModel>>("api/lecturers");
        ViewBag.Courses = new SelectList(courses?.Value ?? new List<CourseViewModel>(), "CourseID", "CourseName");
        ViewBag.Lecturers = new SelectList(lecturers?.Value ?? new List<LecturerViewModel>(), "LecturerID", "LecturerName");
    }
}
