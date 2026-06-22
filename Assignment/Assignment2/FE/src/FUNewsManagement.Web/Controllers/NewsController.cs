using FUNewsManagement.Web.Common;
using FUNewsManagement.Web.Models.Dtos;
using FUNewsManagement.Web.Models.ViewModels;
using FUNewsManagement.Web.Services.ApiClients;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FUNewsManagement.Web.Controllers;

[Authorize(Roles = "Staff,Admin")]
public class NewsController : Controller
{
    private readonly INewsApiClient _newsApiClient;
    private readonly ICategoryApiClient _categoryApiClient;
    private readonly ITagApiClient _tagApiClient;

    public NewsController(INewsApiClient newsApiClient, ICategoryApiClient categoryApiClient, ITagApiClient tagApiClient)
    {
        _newsApiClient = newsApiClient;
        _categoryApiClient = categoryApiClient;
        _tagApiClient = tagApiClient;
    }

    public async Task<IActionResult> Manage(string? keyword, int? tagId, bool mineOnly = false)
    {
        var vm = new NewsManageViewModel
        {
            News = await _newsApiClient.GetForManagementAsync(keyword, tagId, mineOnly),
            Categories = await _categoryApiClient.GetTreeAsync(),
            AllTags = await _tagApiClient.GetAllAsync(),
            Keyword = keyword,
            TagId = tagId,
            MineOnly = mineOnly,
        };
        return View(vm);
    }

    /// <summary>JSON lookup used by the Manage page to populate the Edit modal via fetch().</summary>
    [HttpGet]
    public async Task<IActionResult> GetJson(string id)
    {
        var article = await _newsApiClient.GetByIdForManagementAsync(id);
        return article is null ? NotFound() : Json(article);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(NewsArticleUpsertDto news, string? newTagNamesInput)
    {
        ApplyNewTagNames(news, newTagNamesInput);

        try
        {
            await _newsApiClient.CreateAsync(news);
        }
        catch (ApiException ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Manage));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string id, NewsArticleUpsertDto news, string? newTagNamesInput)
    {
        ApplyNewTagNames(news, newTagNamesInput);

        try
        {
            await _newsApiClient.UpdateAsync(id, news);
        }
        catch (ApiException ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Manage));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Approve(string id)
    {
        try
        {
            await _newsApiClient.ApproveAsync(id);
        }
        catch (ApiException ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Manage));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(string id)
    {
        try
        {
            await _newsApiClient.DeleteAsync(id);
        }
        catch (ApiException ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Manage));
    }

    private static void ApplyNewTagNames(NewsArticleUpsertDto news, string? newTagNamesInput)
    {
        if (string.IsNullOrWhiteSpace(newTagNamesInput)) return;

        news.NewTagNames = newTagNamesInput
            .Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
            .ToList();
    }
}
