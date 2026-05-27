namespace DemoValidationRouting.Repository
{
    public interface IRepositoryManager
    {
        Interface.ICompanyRepository Company { get; }
        Interface.IEmployeeRepository Employee { get; }

        void Save();
    }
}
