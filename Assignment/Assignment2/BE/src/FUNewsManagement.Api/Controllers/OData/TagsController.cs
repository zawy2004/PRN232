using FUNewsManagement.BusinessLogic.Dtos;
using FUNewsManagement.BusinessLogic.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace FUNewsManagement.Api.Controllers.OData;

/// <summary>Public, query-composable read endpoint (odata/Tags) used for SEO tag pickers and search.</summary>
[AllowAnonymous]
public class TagsController : ODataController
{
    private readonly ITagService _tagService;

    public TagsController(ITagService tagService)
    {
        _tagService = tagService;
    }

    [EnableQuery]
    public async Task<ActionResult<IQueryable<TagDto>>> Get()
    {
        var tags = await _tagService.GetAllAsync();
        return Ok(tags.AsQueryable());
    }

    [EnableQuery]
    public async Task<ActionResult<TagDto>> Get([FromRoute] int key)
    {
        var tag = await _tagService.GetByIdAsync(key);
        return tag is null ? NotFound() : Ok(tag);
    }
}
