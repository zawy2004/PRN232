using FUNewsManagement.BusinessLogic.Dtos;
using FUNewsManagement.BusinessLogic.Exceptions;
using FUNewsManagement.DataAccess.Entities;
using FUNewsManagement.DataAccess.UnitOfWork;

namespace FUNewsManagement.BusinessLogic.Services;

public class TagService : ITagService
{
    private readonly IUnitOfWork _unitOfWork;

    public TagService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<TagDto>> GetAllAsync()
    {
        var tags = await _unitOfWork.Tags.GetAllAsync();
        return tags.Select(MapEntity).ToList();
    }

    public async Task<TagDto?> GetByIdAsync(int id)
    {
        var tag = await _unitOfWork.Tags.GetByIdAsync(id);
        return tag is null ? null : MapEntity(tag);
    }

    public async Task<TagDto> CreateAsync(TagUpsertDto dto)
    {
        var entity = new Tag
        {
            TagId = await _unitOfWork.Tags.GetNextIdAsync(),
            TagName = dto.TagName,
            Note = dto.Note,
        };

        await _unitOfWork.Tags.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();

        return MapEntity(entity);
    }

    public async Task UpdateAsync(int id, TagUpsertDto dto)
    {
        var entity = await _unitOfWork.Tags.GetByIdAsync(id)
            ?? throw new EntityNotFoundException($"Tag {id} not found.");

        entity.TagName = dto.TagName;
        entity.Note = dto.Note;

        _unitOfWork.Tags.Update(entity);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _unitOfWork.Tags.GetByIdAsync(id)
            ?? throw new EntityNotFoundException($"Tag {id} not found.");

        if (await _unitOfWork.Tags.IsUsedByNewsArticleAsync(id))
            throw new BusinessRuleException("Cannot delete a tag that is attached to a news article.");

        _unitOfWork.Tags.Remove(entity);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<TagDto> GetOrCreateByNameAsync(string tagName)
    {
        var existing = await _unitOfWork.Tags.GetByNameAsync(tagName);
        if (existing is not null) return MapEntity(existing);

        return await CreateAsync(new TagUpsertDto { TagName = tagName });
    }

    private static TagDto MapEntity(Tag t) => new()
    {
        TagId = t.TagId,
        TagName = t.TagName ?? string.Empty,
        Note = t.Note,
    };
}
