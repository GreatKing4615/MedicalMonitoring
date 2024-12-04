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

            CreateMap<ResearchHistory, PatientFlowData>()
                .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.ResearchDate.Date))
                .ForMember(dest => dest.PatientCount, opt => opt.MapFrom(src => 1));

            CreateMap<SimulationResult, SimulationResultDto>()
            .ForMember(dest => dest.DeviceModelName, opt => opt.MapFrom(src => src.Device.ModelName));
        }
    }

}
