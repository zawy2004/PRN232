using AutoMapper;
using DemoValidationRouting.Dtos;
using DemoValidationRouting.Models;

namespace DemoValidationRouting.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Company, CompanyDto>()
                .ForMember(dest => dest.FullAddress, opt => opt.MapFrom(src => src.Address + ", " + src.Country));

            CreateMap<Employee, EmployeeDto>();

            CreateMap<CompanyForCreationDto, Company>();
            CreateMap<CompanyForUpdateDto, Company>();

            CreateMap<EmployeeForCreationDto, Employee>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());
            CreateMap<EmployeeForUpdateDto, Employee>().ReverseMap();
        }
    }
}
