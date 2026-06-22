using FUNewsManagement.BusinessLogic.Common;
using FUNewsManagement.BusinessLogic.Dtos;
using FUNewsManagement.BusinessLogic.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FUNewsManagement.Api.Controllers;

[ApiController]
[Route("api/categories")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoriesController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<List<CategoryDto>>> GetAll() => Ok(await _categoryService.GetAllAsync());

    /// <summary>Hierarchical (parent/child) tree backing the multi-level navigation menu.</summary>
    [HttpGet("tree")]
    [AllowAnonymous]
    public async Task<ActionResult<List<CategoryTreeDto>>> GetTree() => Ok(await _categoryService.GetTreeAsync());

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<CategoryDto>> GetById(short id)
    {
        var category = await _categoryService.GetByIdAsync(id);
        return category is null ? NotFound() : Ok(category);
    }

    [HttpPost]
    [Authorize(Roles = RoleNames.StaffOrAdmin)]
    public async Task<ActionResult<CategoryDto>> Create(CategoryUpsertDto dto)
    {
        var created = await _categoryService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.CategoryId }, created);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = RoleNames.StaffOrAdmin)]
    public async Task<IActionResult> Update(short id, CategoryUpsertDto dto)
    {
        await _categoryService.UpdateAsync(id, dto);
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = RoleNames.StaffOrAdmin)]
    public async Task<IActionResult> Delete(short id)
    {
        await _categoryService.DeleteAsync(id);
        return NoContent();
    }
}
