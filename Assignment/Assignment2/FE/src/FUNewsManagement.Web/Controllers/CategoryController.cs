using FUNewsManagement.Web.Common;
using FUNewsManagement.Web.Models.Dtos;
using FUNewsManagement.Web.Services.ApiClients;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FUNewsManagement.Web.Controllers;

[Authorize(Roles = "Staff,Admin")]
public class CategoryController : Controller
{
    private readonly ICategoryApiClient _categoryApiClient;

    public CategoryController(ICategoryApiClient categoryApiClient)
    {
        _categoryApiClient = categoryApiClient;
    }

    public async Task<IActionResult> Index()
    {
        var categories = await _categoryApiClient.GetAllAsync();
        ViewBag.AllCategories = categories;
        return View(categories);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CategoryUpsertDto model)
    {
        try
        {
            await _categoryApiClient.CreateAsync(model);
        }
        catch (ApiException ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(short id, CategoryUpsertDto model)
    {
        try
        {
            await _categoryApiClient.UpdateAsync(id, model);
        }
        catch (ApiException ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(short id)
    {
        try
        {
            await _categoryApiClient.DeleteAsync(id);
        }
        catch (ApiException ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }
}
