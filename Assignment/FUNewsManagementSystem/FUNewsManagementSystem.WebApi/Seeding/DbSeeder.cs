using FUNewsManagementSystem.DataAccess;
using FUNewsManagementSystem.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace FUNewsManagementSystem.WebApi.Seeding;

public static class DbSeeder
{
    public static async Task SeedAdminAsync(FunewsDbContext context, AdminAccountOptions adminOptions)
    {
        if (string.IsNullOrWhiteSpace(adminOptions.Email))
        {
            return;
        }

        var existingAdmin = await context.SystemAccounts
            .FirstOrDefaultAsync(a => a.AccountEmail == adminOptions.Email);

        if (existingAdmin is not null)
        {
            return;
        }

        var adminAccount = new SystemAccount
        {
            AccountName = string.IsNullOrWhiteSpace(adminOptions.Name) ? "Administrator" : adminOptions.Name,
            AccountEmail = adminOptions.Email,
            AccountPassword = adminOptions.Password,
            AccountRole = adminOptions.Role
        };

        context.SystemAccounts.Add(adminAccount);
        await context.SaveChangesAsync();
    }

    public static async Task SeedMockDataAsync(FunewsDbContext context, AdminAccountOptions adminOptions)
    {
        if (await context.SystemAccounts.AnyAsync(a => a.AccountEmail != adminOptions.Email))
        {
            return;
        }

        var staffAccounts = new List<SystemAccount>
        {
            new()
            {
                AccountName = "Nguyen Van A",
                AccountEmail = "staff1@funews.edu",
                AccountPassword = "123456",
                AccountRole = 1
            },
            new()
            {
                AccountName = "Tran Thi B",
                AccountEmail = "staff2@funews.edu",
                AccountPassword = "123456",
                AccountRole = 1
            },
            new()
            {
                AccountName = "Le Van C",
                AccountEmail = "lecturer1@funews.edu",
                AccountPassword = "123456",
                AccountRole = 2
            }
        };

        context.SystemAccounts.AddRange(staffAccounts);
        await context.SaveChangesAsync();

        if (!await context.Categories.AnyAsync())
        {
            var categories = new List<Category>
            {
                new()
                {
                    CategoryName = "Campus News",
                    CategoryDescription = "Updates from campus",
                    IsActive = true
                },
                new()
                {
                    CategoryName = "Research",
                    CategoryDescription = "Research achievements and grants",
                    IsActive = true
                },
                new()
                {
                    CategoryName = "Student Life",
                    CategoryDescription = "Student activities and clubs",
                    IsActive = true
                },
                new()
                {
                    CategoryName = "Archive",
                    CategoryDescription = "Inactive news",
                    IsActive = false
                }
            };

            context.Categories.AddRange(categories);
            await context.SaveChangesAsync();
        }

        if (!await context.Tags.AnyAsync())
        {
            var tags = new List<Tag>
            {
                new() { TagName = "Announcement", Note = "General updates" },
                new() { TagName = "Scholarship", Note = "Student funding" },
                new() { TagName = "Event", Note = "On-campus events" },
                new() { TagName = "Research", Note = "Research related" },
                new() { TagName = "Workshop", Note = "Training session" }
            };

            context.Tags.AddRange(tags);
            await context.SaveChangesAsync();
        }

        if (await context.NewsArticles.AnyAsync())
        {
            return;
        }

        var categoriesLookup = await context.Categories.ToListAsync();
        var tagsLookup = await context.Tags.ToListAsync();
        var staffLookup = await context.SystemAccounts
            .Where(a => a.AccountEmail != adminOptions.Email)
            .ToListAsync();

        var staff1 = staffLookup[0];
        var staff2 = staffLookup[1];
        var lecturer = staffLookup[2];

        var campusCategory = categoriesLookup.First(c => c.CategoryName == "Campus News");
        var researchCategory = categoriesLookup.First(c => c.CategoryName == "Research");
        var studentCategory = categoriesLookup.First(c => c.CategoryName == "Student Life");
        var archiveCategory = categoriesLookup.First(c => c.CategoryName == "Archive");

        var announcementTag = tagsLookup.First(t => t.TagName == "Announcement");
        var eventTag = tagsLookup.First(t => t.TagName == "Event");
        var scholarshipTag = tagsLookup.First(t => t.TagName == "Scholarship");
        var researchTag = tagsLookup.First(t => t.TagName == "Research");
        var workshopTag = tagsLookup.First(t => t.TagName == "Workshop");

        var articles = new List<NewsArticle>
        {
            new()
            {
                NewsTitle = "Campus safety updates for summer",
                Headline = "New procedures for after-hours access",
                NewsContent = "The university has updated access rules for summer term. Please carry your student card after 8 PM.",
                NewsSource = "Campus Office",
                CategoryId = campusCategory.CategoryId,
                NewsStatus = true,
                CreatedById = staff1.AccountId,
                CreatedDate = DateTime.UtcNow.AddDays(-10)
            },
            new()
            {
                NewsTitle = "Research lab receives national grant",
                Headline = "Engineering faculty awarded $2M grant",
                NewsContent = "The Advanced Materials Lab has been awarded a national grant to expand its research program.",
                NewsSource = "Research Office",
                CategoryId = researchCategory.CategoryId,
                NewsStatus = true,
                CreatedById = lecturer.AccountId,
                CreatedDate = DateTime.UtcNow.AddDays(-5)
            },
            new()
            {
                NewsTitle = "Student clubs fair opens this weekend",
                Headline = "Over 40 clubs will be present",
                NewsContent = "Join us for the annual clubs fair with performances, booths, and sign-ups.",
                NewsSource = "Student Affairs",
                CategoryId = studentCategory.CategoryId,
                NewsStatus = true,
                CreatedById = staff2.AccountId,
                CreatedDate = DateTime.UtcNow.AddDays(-2)
            },
            new()
            {
                NewsTitle = "Archived: Spring workshop summary",
                Headline = "Highlights from the spring training",
                NewsContent = "This workshop covered project management basics and team collaboration tools.",
                NewsSource = "Training Center",
                CategoryId = archiveCategory.CategoryId,
                NewsStatus = false,
                CreatedById = staff1.AccountId,
                CreatedDate = DateTime.UtcNow.AddDays(-60)
            }
        };

        context.NewsArticles.AddRange(articles);
        await context.SaveChangesAsync();

        var tagMap = new List<NewsTag>
        {
            new() { NewsArticleId = articles[0].NewsArticleId, TagId = announcementTag.TagId },
            new() { NewsArticleId = articles[0].NewsArticleId, TagId = eventTag.TagId },
            new() { NewsArticleId = articles[1].NewsArticleId, TagId = researchTag.TagId },
            new() { NewsArticleId = articles[1].NewsArticleId, TagId = workshopTag.TagId },
            new() { NewsArticleId = articles[2].NewsArticleId, TagId = eventTag.TagId },
            new() { NewsArticleId = articles[2].NewsArticleId, TagId = scholarshipTag.TagId },
            new() { NewsArticleId = articles[3].NewsArticleId, TagId = workshopTag.TagId }
        };

        context.NewsTags.AddRange(tagMap);
        await context.SaveChangesAsync();
    }
}
