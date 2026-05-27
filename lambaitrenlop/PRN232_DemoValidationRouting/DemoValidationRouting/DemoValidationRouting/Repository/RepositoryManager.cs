using DemoValidationRouting.Interface;
using DemoValidationRouting.Data;

namespace DemoValidationRouting.Repository
{
    public class RepositoryManager: IRepositoryManager
    {
        private AppDbContext _dbContext;
        private ICompanyRepository _companyRepository;
        private IEmployeeRepository _employeeRepository;

        public RepositoryManager(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public ICompanyRepository Company
        {
            get
            {
                if (_companyRepository == null)
                {
                    _companyRepository = new CompanyRepository(_dbContext);
                }
                return _companyRepository;
            }
        }

        public IEmployeeRepository Employee
        {
            get
            {
                if (_employeeRepository == null)
                {
                    _employeeRepository = new EmployeeRepository(_dbContext);
                }
                return _employeeRepository;
            }
        }

        public void Save()
        {
            _dbContext.SaveChanges();
        }
    }
}
