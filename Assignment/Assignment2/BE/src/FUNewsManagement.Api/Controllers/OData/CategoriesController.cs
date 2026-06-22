using FUNewsManagement.BusinessLogic.Dtos;
using FUNewsManagement.BusinessLogic.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace FUNewsManagement.Api.Controllers.OData;

/// <summary>Public, query-composable read endpoint (odata/Categories) backing the multi-level menu and category pickers.</summary>
[AllowAnonymous]
public class CategoriesController : ODataController
{
    private readonly ICategoryService _categoryService;

    public CategoriesController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [EnableQuery]
    public async Task<ActionResult<IQueryable<CategoryDto>>> Get()
    {
        var categories = await _categoryService.GetAllAsync();
        return Ok(categories.AsQueryable());
    }

    [EnableQuery]
    public async Task<ActionResult<CategoryDto>> Get([FromRoute] short key)
    {
        var category = await _categoryService.GetByIdAsync(key);
        return category is null ? NotFound() : Ok(category);
    }
}
