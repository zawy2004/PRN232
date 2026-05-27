using DemoValidationRouting.Interface;
using DemoValidationRouting.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace DemoValidationRouting.Repository
{
    public class CompanyRepository : RepositoryBase<Company>, ICompanyRepository
    {
        public CompanyRepository(AppDbContext repositoryContext) : base(repositoryContext)
        {
        }

        public IEnumerable<Company> GetAllCompanies(bool trackChanges) =>
            FindAll(trackChanges).OrderBy(c => c.Name).ToList();

        public Company GetCompany(Guid companyId, bool trackChanges) =>
            FindByCondition(c => c.Id.Equals(companyId), trackChanges).SingleOrDefault();

        public void CreateCompany(Company company) => Create(company);

        public IEnumerable<Company> GetByIds(IEnumerable<Guid> ids, bool trackChanges) =>
            FindByCondition(c => ids.Contains(c.Id), trackChanges).ToList();
    }
}
