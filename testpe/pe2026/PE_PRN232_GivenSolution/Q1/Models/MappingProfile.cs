using AutoMapper;
using Q1.DTOs;

namespace Q1.Models
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Equipment, EquipmentDto>().ForMember(d => d.UploaderEmail, o => o.MapFrom(s => s.User.Email));
            CreateMap<CreateEquipmentDto, Equipment>();
            CreateMap<Rental, RentalDto>().ForMember(d => d.EquipmentName, o => o.MapFrom(s => s.Equipment.Name));
        }
    }
}
