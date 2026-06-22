using FUNewsManagement.Web.Common;
using FUNewsManagement.Web.Models.Dtos;
using FUNewsManagement.Web.Services.ApiClients;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FUNewsManagement.Web.Controllers;

[Authorize(Roles = "Admin")]
public class TagController : Controller
{
    private readonly ITagApiClient _tagApiClient;

    public TagController(ITagApiClient tagApiClient)
    {
        _tagApiClient = tagApiClient;
    }

    public async Task<IActionResult> Index()
    {
        var tags = await _tagApiClient.GetAllAsync();
        return View(tags);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TagUpsertDto model)
    {
        try
        {
            await _tagApiClient.CreateAsync(model);
        }
        catch (ApiException ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, TagUpsertDto model)
    {
        try
        {
            await _tagApiClient.UpdateAsync(id, model);
        }
        catch (ApiException ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _tagApiClient.DeleteAsync(id);
        }
        catch (ApiException ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }
}
