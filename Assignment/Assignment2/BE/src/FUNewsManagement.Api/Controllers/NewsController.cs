using FUNewsManagement.Api.Common;
using FUNewsManagement.BusinessLogic.Common;
using FUNewsManagement.BusinessLogic.Dtos;
using FUNewsManagement.BusinessLogic.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FUNewsManagement.Api.Controllers;

[ApiController]
[Route("api/news")]
[Authorize(Roles = RoleNames.StaffOrAdmin)]
public class NewsController : ControllerBase
{
    private readonly INewsService _newsService;

    public NewsController(INewsService newsService)
    {
        _newsService = newsService;
    }

    [HttpGet("manage")]
    public async Task<ActionResult<List<NewsArticleDto>>> GetForManagement(
        [FromQuery] string? keyword, [FromQuery] int? tagId, [FromQuery] bool mineOnly = false)
    {
        short? createdById = mineOnly ? this.CurrentAccountId() : null;
        var news = await _newsService.GetForManagementAsync(keyword, tagId, createdById);
        return Ok(news);
    }

    [HttpGet("manage/{id}")]
    public async Task<ActionResult<NewsArticleDto>> GetById(string id)
    {
        var article = await _newsService.GetByIdForManagementAsync(id);
        return article is null ? NotFound() : Ok(article);
    }

    [HttpPost]
    public async Task<ActionResult<NewsArticleDto>> Create(NewsArticleUpsertDto dto)
    {
        var created = await _newsService.CreateAsync(dto, this.CurrentAccountId());
        return CreatedAtAction(nameof(GetById), new { id = created.NewsArticleId }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, NewsArticleUpsertDto dto)
    {
        await _newsService.UpdateAsync(id, dto, this.CurrentAccountId());
        return NoContent();
    }

    /// <summary>Admin-only: approve a pending article so it becomes visible to guests (and locked from edits).</summary>
    [HttpPost("{id}/approve")]
    [Authorize(Roles = RoleNames.Admin)]
    public async Task<IActionResult> Approve(string id)
    {
        await _newsService.ApproveAsync(id);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        await _newsService.DeleteAsync(id);
        return NoContent();
    }
}
