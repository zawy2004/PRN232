using FUNewsManagementSystem.DataAccess.Entities;
using FUNewsManagementSystem.Repositories.Interfaces;
using FUNewsManagementSystem.WebApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace FUNewsManagementSystem.WebApi.Controllers.OData;

[ApiController]
[Route("odata")]
public class NewsArticlesController : ODataController
{
    private readonly INewsArticleRepository _repository;

    public NewsArticlesController(INewsArticleRepository repository)
    {
        _repository = repository;
    }

    [EnableQuery]
    [HttpGet("NewsArticles")]
    public async Task<IActionResult> Get()
    {
        var news = await _repository.GetAllAsync(includeTags: true);
        return Ok(news);
    }

    [EnableQuery]
    [HttpGet("NewsArticles({key})")]
    public async Task<IActionResult> Get([FromRoute] int key)
    {
        var article = await _repository.GetByIdAsync(key, includeTags: true);
        if (article is null)
        {
            return NotFound();
        }

        return Ok(article);
    }

    [HttpGet("/api/news/active")]
    public async Task<IActionResult> GetActive()
    {
        var news = await _repository.GetAllAsync(includeTags: true);
        var activeNews = news.Where(n => n.NewsStatus).ToList();
        return Ok(activeNews);
    }

    [HttpGet("/api/news/creator/{accountId:int}")]
    public async Task<IActionResult> GetByCreator([FromRoute] int accountId)
    {
        var news = await _repository.GetByCreatorAsync(accountId);
        return Ok(news);
    }

    [HttpPost("NewsArticles")]
    public async Task<IActionResult> Post([FromBody] NewsArticleCreateDto request)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var article = new NewsArticle
        {
            NewsTitle = request.NewsTitle,
            Headline = request.Headline,
            NewsContent = request.NewsContent,
            NewsSource = request.NewsSource,
            CategoryId = request.CategoryId,
            NewsStatus = request.NewsStatus,
            CreatedById = request.CreatedById,
            CreatedDate = DateTime.UtcNow
        };

        try
        {
            var created = await _repository.AddAsync(article, request.TagIds.ToList());
            return Created(created);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("NewsArticles({key})")]
    public async Task<IActionResult> Put([FromRoute] int key, [FromBody] NewsArticleUpdateDto request)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var article = new NewsArticle
        {
            NewsArticleId = key,
            NewsTitle = request.NewsTitle,
            Headline = request.Headline,
            NewsContent = request.NewsContent,
            NewsSource = request.NewsSource,
            CategoryId = request.CategoryId,
            NewsStatus = request.NewsStatus,
            UpdatedById = request.UpdatedById,
            ModifiedDate = DateTime.UtcNow
        };

        try
        {
            var updated = await _repository.UpdateAsync(key, article, request.TagIds.ToList());
            if (!updated)
            {
                return NotFound();
            }

            return Updated(article);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("NewsArticles({key})")]
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
