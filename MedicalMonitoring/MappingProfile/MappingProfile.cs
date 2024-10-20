using AutoMapper;
using BL.Dtos;
using DAL.Entities;

namespace MedicalMonitoring.MappingProfile
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Device, DeviceDto>().ReverseMap();
            CreateMap<Research, ResearchDto>().ReverseMap();
            CreateMap<User, UserDto>().ReverseMap();
            CreateMap<ResearchHistory, ResearchHistoryDto>().ReverseMap();
            CreateMap<ServiceHistory, ServiceHistoryDto>().ReverseMap();
        }
    }

}
