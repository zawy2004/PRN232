using FUNewsManagement.BusinessLogic.Common;
using FUNewsManagement.BusinessLogic.Dtos;
using FUNewsManagement.BusinessLogic.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FUNewsManagement.Api.Controllers;

[ApiController]
[Route("api/tags")]
public class TagsController : ControllerBase
{
    private readonly ITagService _tagService;

    public TagsController(ITagService tagService)
    {
        _tagService = tagService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<List<TagDto>>> GetAll() => Ok(await _tagService.GetAllAsync());

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<TagDto>> GetById(int id)
    {
        var tag = await _tagService.GetByIdAsync(id);
        return tag is null ? NotFound() : Ok(tag);
    }

    /// <summary>Used by Staff while authoring news to quick-add a new SEO tag inline.</summary>
    [HttpPost("get-or-create")]
    [Authorize(Roles = RoleNames.StaffOrAdmin)]
    public async Task<ActionResult<TagDto>> GetOrCreate([FromBody] string tagName) =>
        Ok(await _tagService.GetOrCreateByNameAsync(tagName));

    [HttpPost]
    [Authorize(Roles = RoleNames.Admin)]
    public async Task<ActionResult<TagDto>> Create(TagUpsertDto dto)
    {
        var created = await _tagService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.TagId }, created);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = RoleNames.Admin)]
    public async Task<IActionResult> Update(int id, TagUpsertDto dto)
    {
        await _tagService.UpdateAsync(id, dto);
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = RoleNames.Admin)]
    public async Task<IActionResult> Delete(int id)
    {
        await _tagService.DeleteAsync(id);
        return NoContent();
    }
}
