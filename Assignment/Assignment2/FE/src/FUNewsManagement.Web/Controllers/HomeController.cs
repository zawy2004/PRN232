using System.Diagnostics;
using FUNewsManagement.Web.Models;
using FUNewsManagement.Web.Models.ViewModels;
using FUNewsManagement.Web.Services.ApiClients;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FUNewsManagement.Web.Controllers;

[AllowAnonymous]
public class HomeController : Controller
{
    private readonly INewsApiClient _newsApiClient;
    private readonly ITagApiClient _tagApiClient;

    public HomeController(INewsApiClient newsApiClient, ITagApiClient tagApiClient)
    {
        _newsApiClient = newsApiClient;
        _tagApiClient = tagApiClient;
    }

    public async Task<IActionResult> Index(string? keyword, int? tagId, short? categoryId)
    {
        var vm = new NewsListViewModel
        {
            News = await _newsApiClient.GetActiveAsync(keyword, tagId, categoryId),
            AllTags = await _tagApiClient.GetAllAsync(),
            Keyword = keyword,
            TagId = tagId,
            CategoryId = categoryId,
        };
        return View(vm);
    }

    public async Task<IActionResult> Details(string id)
    {
        var article = await _newsApiClient.GetActiveByIdAsync(id);
        return article is null ? NotFound() : View(article);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
