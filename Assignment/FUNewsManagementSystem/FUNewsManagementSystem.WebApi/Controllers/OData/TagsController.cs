using FUNewsManagementSystem.DataAccess.Entities;
using FUNewsManagementSystem.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace FUNewsManagementSystem.WebApi.Controllers.OData;

[ApiController]
[Route("odata")]
public class TagsController : ODataController
{
    private readonly ITagRepository _repository;

    public TagsController(ITagRepository repository)
    {
        _repository = repository;
    }

    [EnableQuery]
    [HttpGet("Tags")]
    public async Task<IActionResult> Get()
    {
        var tags = await _repository.GetAllAsync();
        return Ok(tags);
    }

    [EnableQuery]
    [HttpGet("Tags({key})")]
    public async Task<IActionResult> Get([FromRoute] int key)
    {
        var tag = await _repository.GetByIdAsync(key);
        if (tag is null)
        {
            return NotFound();
        }

        return Ok(tag);
    }

    [HttpPost("Tags")]
    public async Task<IActionResult> Post([FromBody] Tag tag)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var created = await _repository.AddAsync(tag);
        return Created(created);
    }

    [HttpPut("Tags({key})")]
    public async Task<IActionResult> Put([FromRoute] int key, [FromBody] Tag tag)
    {
        if (key != tag.TagId)
        {
            return BadRequest("TagId does not match key.");
        }

        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var updated = await _repository.UpdateAsync(tag);
        if (!updated)
        {
            return NotFound();
        }

        return Updated(tag);
    }

    [HttpDelete("Tags({key})")]
    public async Task<IActionResult> Delete([FromRoute] int key)
    {
        var deleted = await _repository.DeleteAsync(key);
        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}
