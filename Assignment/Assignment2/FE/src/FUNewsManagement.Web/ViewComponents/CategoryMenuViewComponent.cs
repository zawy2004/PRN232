using FUNewsManagement.Web.Services.ApiClients;
using Microsoft.AspNetCore.Mvc;

namespace FUNewsManagement.Web.ViewComponents;

public class CategoryMenuViewComponent : ViewComponent
{
    private readonly ICategoryApiClient _categoryApiClient;

    public CategoryMenuViewComponent(ICategoryApiClient categoryApiClient)
    {
        _categoryApiClient = categoryApiClient;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var tree = await _categoryApiClient.GetTreeAsync();
        return View(tree.Where(c => c.IsActive).ToList());
    }
}
