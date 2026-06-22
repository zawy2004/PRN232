using FUNewsManagement.BusinessLogic.Dtos;
using FUNewsManagement.BusinessLogic.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace FUNewsManagement.Api.Controllers.OData;

/// <summary>
/// Public, query-composable read endpoint (odata/NewsArticles) for guest browsing.
/// Only ever exposes active news; $filter/$orderby/$expand power keyword and SEO-tag search,
/// e.g. odata/NewsArticles?$filter=contains(NewsTitle,'AI') or Tags/any(t: t/TagName eq 'Education').
/// </summary>
[AllowAnonymous]
public class NewsArticlesController : ODataController
{
    private readonly INewsService _newsService;

    public NewsArticlesController(INewsService newsService)
    {
        _newsService = newsService;
    }

    [EnableQuery]
    public async Task<ActionResult<IQueryable<NewsArticleDto>>> Get()
    {
        var news = await _newsService.GetActiveAsync(keyword: null, tagId: null);
        return Ok(news.AsQueryable());
    }

    [EnableQuery]
    public async Task<ActionResult<NewsArticleDto>> Get([FromRoute] string key)
    {
        var article = await _newsService.GetActiveByIdAsync(key);
        return article is null ? NotFound() : Ok(article);
    }
}
