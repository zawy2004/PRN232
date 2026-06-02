using FUCourseManagementSystem.WebClient.Models;
using FUCourseManagementSystem.WebClient.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FUCourseManagementSystem.WebClient.Controllers;

public class CoursesController : Controller
{
    private readonly ApiService _api;

    public CoursesController(ApiService api) => _api = api;

    public async Task<IActionResult> Index(string? search)
    {
        var url = "api/courses";
        if (!string.IsNullOrWhiteSpace(search))
            url += $"?$filter=contains(CourseCode,'{search}') or contains(CourseName,'{search}')";

        var result = await _api.GetAsync<ODataResult<CourseViewModel>>(url);
        ViewBag.Search = search;
        return View(result?.Value ?? new List<CourseViewModel>());
    }

    public async Task<IActionResult> Details(int id)
    {
        var course = await _api.GetAsync<CourseViewModel>($"api/courses/{id}");
        return course == null ? NotFound() : View(course);
    }

    [Authorize(Roles = "AcademicStaff,Admin")]
    public async Task<IActionResult> Create()
    {
        await LoadDepartments();
        return View(new CourseViewModel());
    }

    [HttpPost, Authorize(Roles = "AcademicStaff,Admin")]
    public async Task<IActionResult> Create(CourseViewModel model)
    {
        if (!ModelState.IsValid) { await LoadDepartments(); return View(model); }
        var response = await _api.PostAsync("api/courses", model);
        if (response.IsSuccessStatusCode) return RedirectToAction(nameof(Index));
        ModelState.AddModelError("", "Failed to create course.");
        await LoadDepartments();
        return View(model);
    }

    [Authorize(Roles = "AcademicStaff,Admin")]
    public async Task<IActionResult> Edit(int id)
    {
        var course = await _api.GetAsync<CourseViewModel>($"api/courses/{id}");
        if (course == null) return NotFound();
        await LoadDepartments();
        return View(course);
    }

    [HttpPost, Authorize(Roles = "AcademicStaff,Admin")]
    public async Task<IActionResult> Edit(int id, CourseViewModel model)
    {
        if (!ModelState.IsValid) { await LoadDepartments(); return View(model); }
        var response = await _api.PutAsync($"api/courses/{id}", model);
        if (response.IsSuccessStatusCode) return RedirectToAction(nameof(Index));
        ModelState.AddModelError("", "Failed to update course.");
        await LoadDepartments();
        return View(model);
    }

    [HttpPost, Authorize(Roles = "AcademicStaff,Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var response = await _api.DeleteAsync($"api/courses/{id}");
        if (!response.IsSuccessStatusCode)
            TempData["Error"] = "Cannot delete course with existing class sections.";
        return RedirectToAction(nameof(Index));
    }

    private async Task LoadDepartments()
    {
        var depts = await _api.GetAsync<ODataResult<DepartmentViewModel>>("api/departments");
        ViewBag.Departments = new SelectList(depts?.Value ?? new List<DepartmentViewModel>(), "DepartmentID", "DepartmentName");
    }
}
