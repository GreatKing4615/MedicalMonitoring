using AutoMapper;
using BL.Dtos;
using DAL.Repositories;

namespace BL.Services
{
    public interface IDeviceService
    {
        Task<IEnumerable<DeviceDto>> GetDevicesAsync(int pageNumber, int pageSize);
        Task<DeviceDto> GetDeviceByIdAsync(int id);
    }

    public class DeviceService : IDeviceService
    {
        private readonly IDeviceRepository _deviceRepository;
        private readonly IMapper _mapper;

        public DeviceService(IDeviceRepository deviceRepository, IMapper mapper)
        {
            _deviceRepository = deviceRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<DeviceDto>> GetDevicesAsync(int pageNumber, int pageSize)
        {
            var devices = await _deviceRepository.GetDevicesAsync(pageNumber, pageSize);
            return _mapper.Map<IEnumerable<DeviceDto>>(devices);
        }

        public async Task<DeviceDto> GetDeviceByIdAsync(int id)
        {
            var device = await _deviceRepository.GetDeviceByIdAsync(id);
            if (device == null)
            {
                throw new KeyNotFoundException("Device not found");
            }
            return _mapper.Map<DeviceDto>(device);
        }
    }

}