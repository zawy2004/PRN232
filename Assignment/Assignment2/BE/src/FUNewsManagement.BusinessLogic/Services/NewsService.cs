using FUNewsManagement.BusinessLogic.Dtos;
using FUNewsManagement.BusinessLogic.Exceptions;
using FUNewsManagement.DataAccess.Entities;
using FUNewsManagement.DataAccess.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace FUNewsManagement.BusinessLogic.Services;

public class NewsService : INewsService
{
    private readonly IUnitOfWork _unitOfWork;

    public NewsService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<NewsArticleDto>> GetActiveAsync(string? keyword, int? tagId)
    {
        // NewsStatus == true means "approved" - guests only ever see approved articles.
        var query = ApplySearch(_unitOfWork.NewsArticles.QueryWithDetails().Where(n => n.NewsStatus == true), keyword, tagId);
        var entities = await query.OrderByDescending(n => n.CreatedDate).ToListAsync();
        return entities.Select(MapEntity).ToList();
    }

    public async Task<NewsArticleDto?> GetActiveByIdAsync(string id)
    {
        var entity = await _unitOfWork.NewsArticles.QueryWithDetails()
            .FirstOrDefaultAsync(n => n.NewsArticleId == id && n.NewsStatus == true);
        return entity is null ? null : MapEntity(entity);
    }

    public async Task<List<NewsArticleDto>> GetForManagementAsync(string? keyword, int? tagId, short? createdById)
    {
        var query = _unitOfWork.NewsArticles.QueryWithDetails();
        if (createdById.HasValue)
            query = query.Where(n => n.CreatedById == createdById.Value);

        query = ApplySearch(query, keyword, tagId);

        var entities = await query.OrderByDescending(n => n.CreatedDate).ToListAsync();
        return entities.Select(MapEntity).ToList();
    }

    public async Task<NewsArticleDto?> GetByIdForManagementAsync(string id)
    {
        var entity = await _unitOfWork.NewsArticles.GetWithDetailsAsync(id);
        return entity is null ? null : MapEntity(entity);
    }

    public async Task<NewsArticleDto> CreateAsync(NewsArticleUpsertDto dto, short createdById)
    {
        var entity = new NewsArticle
        {
            NewsArticleId = await _unitOfWork.NewsArticles.GetNextIdAsync(),
            NewsTitle = dto.NewsTitle,
            Headline = dto.Headline,
            NewsContent = dto.NewsContent,
            NewsSource = dto.NewsSource,
            CategoryId = dto.CategoryId,
            NewsStatus = false, // new articles always start pending (unapproved) until an Admin approves them
            CreatedById = createdById,
            CreatedDate = DateTime.UtcNow,
        };

        await AttachTagsAsync(entity, dto);

        await _unitOfWork.NewsArticles.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();

        var created = await _unitOfWork.NewsArticles.GetWithDetailsAsync(entity.NewsArticleId)
            ?? throw new EntityNotFoundException("News article was not persisted.");
        return MapEntity(created);
    }

    public async Task UpdateAsync(string id, NewsArticleUpsertDto dto, short updatedById)
    {
        var entity = await _unitOfWork.NewsArticles.GetWithDetailsAsync(id)
            ?? throw new EntityNotFoundException($"News article {id} not found.");

        if (entity.NewsStatus == true)
            throw new ForbiddenOperationException("This article has been approved and can no longer be edited.");

        entity.NewsTitle = dto.NewsTitle;
        entity.Headline = dto.Headline;
        entity.NewsContent = dto.NewsContent;
        entity.NewsSource = dto.NewsSource;
        entity.CategoryId = dto.CategoryId;
        // NewsStatus (approval) deliberately not changed here - it stays pending until Admin approves.
        entity.UpdatedById = updatedById;
        entity.ModifiedDate = DateTime.UtcNow;

        entity.Tags.Clear();
        await AttachTagsAsync(entity, dto);

        _unitOfWork.NewsArticles.Update(entity);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task ApproveAsync(string id)
    {
        var entity = await _unitOfWork.NewsArticles.GetByIdAsync(id)
            ?? throw new EntityNotFoundException($"News article {id} not found.");

        if (entity.NewsStatus == true)
            throw new BusinessRuleException("This article has already been approved.");

        entity.NewsStatus = true; // approve = publish (visible to guests) and lock from further edits
        _unitOfWork.NewsArticles.Update(entity);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteAsync(string id)
    {
        var entity = await _unitOfWork.NewsArticles.GetWithDetailsAsync(id)
            ?? throw new EntityNotFoundException($"News article {id} not found.");

        entity.Tags.Clear();
        _unitOfWork.NewsArticles.Remove(entity);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<List<ReportItemDto>> GetReportAsync(DateTime start, DateTime end)
    {
        var entities = await _unitOfWork.NewsArticles.QueryWithDetails()
            .Where(n => n.CreatedDate >= start && n.CreatedDate <= end)
            .OrderByDescending(n => n.CreatedDate)
            .ToListAsync();

        return entities.Select(e => new ReportItemDto
        {
            NewsArticleId = e.NewsArticleId,
            NewsTitle = e.NewsTitle,
            CategoryName = e.Category?.CategoryName,
            CreatedByName = e.CreatedBy?.AccountName,
            CreatedDate = e.CreatedDate,
            NewsStatus = e.NewsStatus ?? false,
        }).ToList();
    }

    private async Task AttachTagsAsync(NewsArticle entity, NewsArticleUpsertDto dto)
    {
        foreach (var tagId in dto.TagIds.Distinct())
        {
            var tag = await _unitOfWork.Tags.GetByIdAsync(tagId);
            if (tag is not null) entity.Tags.Add(tag);
        }

        foreach (var tagName in dto.NewTagNames.Where(n => !string.IsNullOrWhiteSpace(n)).Distinct())
        {
            var tag = await _unitOfWork.Tags.GetByNameAsync(tagName);
            if (tag is null)
            {
                tag = new Tag { TagId = await _unitOfWork.Tags.GetNextIdAsync(), TagName = tagName };
                await _unitOfWork.Tags.AddAsync(tag);
            }
            if (!entity.Tags.Any(t => t.TagId == tag.TagId)) entity.Tags.Add(tag);
        }
    }

    private static IQueryable<NewsArticle> ApplySearch(IQueryable<NewsArticle> query, string? keyword, int? tagId)
    {
        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(n =>
                (n.NewsTitle != null && n.NewsTitle.Contains(keyword)) ||
                n.Headline.Contains(keyword) ||
                n.Tags.Any(t => t.TagName != null && t.TagName.Contains(keyword)));
        }

        if (tagId.HasValue)
        {
            query = query.Where(n => n.Tags.Any(t => t.TagId == tagId.Value));
        }

        return query;
    }

    private static NewsArticleDto MapEntity(NewsArticle e) => new()
    {
        NewsArticleId = e.NewsArticleId,
        NewsTitle = e.NewsTitle,
        Headline = e.Headline,
        CreatedDate = e.CreatedDate,
        NewsContent = e.NewsContent,
        NewsSource = e.NewsSource,
        CategoryId = e.CategoryId,
        CategoryName = e.Category?.CategoryName,
        NewsStatus = e.NewsStatus ?? false,
        CreatedById = e.CreatedById,
        CreatedByName = e.CreatedBy?.AccountName,
        UpdatedById = e.UpdatedById,
        ModifiedDate = e.ModifiedDate,
        Tags = e.Tags.Select(t => new TagDto { TagId = t.TagId, TagName = t.TagName ?? string.Empty, Note = t.Note }).ToList(),
        // Editable only while still pending (NewsStatus false); once approved it is locked for everyone.
        CanEdit = !(e.NewsStatus ?? false),
    };
}
