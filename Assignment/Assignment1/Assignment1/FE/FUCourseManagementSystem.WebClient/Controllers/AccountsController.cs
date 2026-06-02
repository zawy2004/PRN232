using FUCourseManagementSystem.WebClient.Models;
using FUCourseManagementSystem.WebClient.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FUCourseManagementSystem.WebClient.Controllers;

[Authorize(Roles = "Admin")]
public class AccountsController : Controller
{
    private readonly ApiService _api;

    public AccountsController(ApiService api) => _api = api;

    public async Task<IActionResult> Index(string? search)
    {
        var url = "api/systemaccounts";
        if (!string.IsNullOrWhiteSpace(search))
            url += $"?$filter=contains(AccountName,'{search}') or contains(AccountEmail,'{search}')";

        var result = await _api.GetAsync<ODataResult<SystemAccountViewModel>>(url);
        ViewBag.Search = search;
        return View(result?.Value ?? new List<SystemAccountViewModel>());
    }

    public async Task<IActionResult> Details(int id)
    {
        var account = await _api.GetAsync<SystemAccountViewModel>($"api/systemaccounts/{id}");
        return account == null ? NotFound() : View(account);
    }

    public IActionResult Create() => View(new SystemAccountViewModel());

    [HttpPost]
    public async Task<IActionResult> Create(SystemAccountViewModel model)
    {
        if (!ModelState.IsValid) return View(model);
        var response = await _api.PostAsync("api/systemaccounts", model);
        if (response.IsSuccessStatusCode) return RedirectToAction(nameof(Index));
        ModelState.AddModelError("", "Failed to create account.");
        return View(model);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var account = await _api.GetAsync<SystemAccountViewModel>($"api/systemaccounts/{id}");
        return account == null ? NotFound() : View(account);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, SystemAccountViewModel model)
    {
        if (!ModelState.IsValid) return View(model);
        var response = await _api.PutAsync($"api/systemaccounts/{id}", model);
        if (response.IsSuccessStatusCode) return RedirectToAction(nameof(Index));
        ModelState.AddModelError("", "Failed to update account.");
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var response = await _api.DeleteAsync($"api/systemaccounts/{id}");
        if (!response.IsSuccessStatusCode)
            TempData["Error"] = "Cannot delete account linked to a lecturer or student.";
        return RedirectToAction(nameof(Index));
    }
}
