using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace DemoValidationRouting.Data
{
    public class EmployeeConfiguration: IEntityTypeConfiguration<Models.Employee>
    {
        public void Configure(EntityTypeBuilder<Models.Employee> builder)
        {
            builder.HasData(
                new Models.Employee
                {
                    Id = Guid.Parse("021ca3c1-0deb-4afd-ae94-2159a8479811"),
                    Name = "John Doe",
                    Age = 30,
                    Position = "Software Engineer",
                    CompanyId = Guid.Parse("d28888e9-2ba9-473a-a40f-e38cb54f9b35")
                },
                new Models.Employee
                {
                    Id = Guid.Parse("021ca3c1-0deb-4afd-ae94-2159a8479812"),
                    Name = "Jane Smith",
                    Age = 28,
                    Position = "Product Manager",
                    CompanyId = Guid.Parse("c9d4c053-49b6-410c-bc78-2d54a9991870")
                },
                new Models.Employee
                {
                    Id = Guid.Parse("021ca3c1-0deb-4afd-ae94-2159a8479813"),
                    Name = "Emily Johnson",
                    Age = 35,
                    Position = "UX Designer",
                    CompanyId = Guid.Parse("3d490a70-94ce-4d15-9494-5248280c2ce3")
                }
            );
        }
    }
}
