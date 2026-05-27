using FUNewsManagementSystem.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace FUNewsManagementSystem.DataAccess.Daos;

public sealed class TagDao
{
    private static readonly Lazy<TagDao> LazyInstance = new(() => new TagDao());

    public static TagDao Instance => LazyInstance.Value;

    private TagDao()
    {
    }

    public async Task<List<Tag>> GetAllAsync()
    {
        using var context = DbContextFactory.Instance.CreateDbContext();
        return await context.Tags.AsNoTracking().ToListAsync();
    }

    public async Task<Tag?> GetByIdAsync(int id)
    {
        using var context = DbContextFactory.Instance.CreateDbContext();
        return await context.Tags.AsNoTracking().FirstOrDefaultAsync(t => t.TagId == id);
    }

    public async Task<Tag> AddAsync(Tag tag)
    {
        using var context = DbContextFactory.Instance.CreateDbContext();
        context.Tags.Add(tag);
        await context.SaveChangesAsync();
        return tag;
    }

    public async Task<bool> UpdateAsync(Tag tag)
    {
        using var context = DbContextFactory.Instance.CreateDbContext();
        var existing = await context.Tags.FirstOrDefaultAsync(t => t.TagId == tag.TagId);
        if (existing is null)
        {
            return false;
        }

        existing.TagName = tag.TagName;
        existing.Note = tag.Note;

        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        using var context = DbContextFactory.Instance.CreateDbContext();
        var existing = await context.Tags.FirstOrDefaultAsync(t => t.TagId == id);
        if (existing is null)
        {
            return false;
        }

        context.Tags.Remove(existing);
        await context.SaveChangesAsync();
        return true;
    }
}
