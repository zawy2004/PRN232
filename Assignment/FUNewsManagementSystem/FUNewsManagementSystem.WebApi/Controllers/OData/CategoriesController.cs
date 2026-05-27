using FUNewsManagementSystem.DataAccess.Entities;
using FUNewsManagementSystem.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace FUNewsManagementSystem.WebApi.Controllers.OData;

[ApiController]
[Route("odata")]
public class CategoriesController : ODataController
{
    private readonly ICategoryRepository _repository;

    public CategoriesController(ICategoryRepository repository)
    {
        _repository = repository;
    }

    [EnableQuery]
    [HttpGet("Categories")]
    public async Task<IActionResult> Get()
    {
        var categories = await _repository.GetAllAsync();
        return Ok(categories);
    }

    [EnableQuery]
    [HttpGet("Categories({key})")]
    public async Task<IActionResult> Get([FromRoute] int key)
    {
        var category = await _repository.GetByIdAsync(key);
        if (category is null)
        {
            return NotFound();
        }

        return Ok(category);
    }

    [HttpPost("Categories")]
    public async Task<IActionResult> Post([FromBody] Category category)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var created = await _repository.AddAsync(category);
        return Created(created);
    }

    [HttpPut("Categories({key})")]
    public async Task<IActionResult> Put([FromRoute] int key, [FromBody] Category category)
    {
        if (key != category.CategoryId)
        {
            return BadRequest("CategoryId does not match key.");
        }

        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var updated = await _repository.UpdateAsync(category);
        if (!updated)
        {
            return NotFound();
        }

        return Updated(category);
    }

    [HttpDelete("Categories({key})")]
    public async Task<IActionResult> Delete([FromRoute] int key)
    {
        var canDelete = await _repository.CanDeleteAsync(key);
        if (!canDelete)
        {
            return BadRequest("Cannot delete category because it is referenced by news articles.");
        }

        var deleted = await _repository.DeleteAsync(key);
        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}
