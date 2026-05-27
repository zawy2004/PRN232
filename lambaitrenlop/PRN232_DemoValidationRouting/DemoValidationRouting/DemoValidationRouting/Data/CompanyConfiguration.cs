using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;


namespace DemoValidationRouting.Data
{
    public class CompanyConfiguration:IEntityTypeConfiguration<Models.Company>
    {
        public void Configure(EntityTypeBuilder<Models.Company> builder)
        {
            builder.HasData(
                new Models.Company
                {
                    Id = Guid.Parse("d28888e9-2ba9-473a-a40f-e38cb54f9b35"),
                    Name = "Microsoft",
                    Address = "One Microsoft Way, Redmond, WA 98052",
                    Country = "USA"
                },
                new Models.Company
                {
                    Id = Guid.Parse("c9d4c053-49b6-410c-bc78-2d54a9991870"),
                    Name = "Google",
                    Address = "1600 Amphitheatre Parkway, Mountain View, CA 94043",
                    Country = "USA"
                },
                new Models.Company
                {
                    Id = Guid.Parse("3d490a70-94ce-4d15-9494-5248280c2ce3"),
                    Name = "Apple",
                    Address = "One Apple Park Way, Cupertino, CA 95014",
                    Country = "USA"
                }
            );
        }
    }
}
