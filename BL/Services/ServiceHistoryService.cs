using AutoMapper;
using BL.Dtos;
using DAL.Entities;
using DAL.Repositories;
using System.ComponentModel.DataAnnotations;

namespace BL.Services
{
    public interface IServiceHistoryService
    {
        Task<IEnumerable<ServiceHistoryDto>> GetServiceHistoryAsync(int pageNumber, int pageSize);
        Task<ServiceHistoryDto> GetServiceHistoryByIdAsync(int id);
        Task<ServiceHistoryDto> CreateServiceHistoryAsync(ServiceHistoryDto serviceHistoryDto);

    }

    public class ServiceHistoryService : IServiceHistoryService
    {
        private readonly IServiceHistoryRepository _serviceHistoryRepository;
        private readonly IDeviceRepository _deviceRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public ServiceHistoryService(IServiceHistoryRepository serviceHistoryRepository, IMapper mapper, IDeviceRepository deviceRepository, IUserRepository userRepository)
        {
            _serviceHistoryRepository = serviceHistoryRepository;
            _mapper = mapper;
            _deviceRepository = deviceRepository;
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<ServiceHistoryDto>> GetServiceHistoryAsync(int pageNumber, int pageSize)
        {
            var history = await _serviceHistoryRepository.GetAllAsync(pageNumber, pageSize);
            return _mapper.Map<IEnumerable<ServiceHistoryDto>>(history);
        }

        public async Task<ServiceHistoryDto> GetServiceHistoryByIdAsync(int id)
        {
            var history = await _serviceHistoryRepository.GetByIdAsync(id);
            if (history == null)
            {
                throw new KeyNotFoundException("Service history not found");
            }
            return _mapper.Map<ServiceHistoryDto>(history);
        }

        public async Task<ServiceHistoryDto> CreateServiceHistoryAsync(ServiceHistoryDto serviceHistoryDto)
        {
            // Business logic: Validate input
            if (serviceHistoryDto.DeviceId == 0)
                throw new ValidationException("DeviceId is required.");

            if (serviceHistoryDto.UserId == 0)
                throw new ValidationException("UserId is required.");

            // Check if Device exists
            var device = await _deviceRepository.GetDeviceByIdAsync(serviceHistoryDto.DeviceId);
            if (device == null)
                throw new ValidationException("Device not found.");

            // Check if User exists
            var user = await _userRepository.GetByIdAsync(serviceHistoryDto.UserId);
            if (user == null)
                throw new ValidationException("User not found.");

            // Map DTO to entity
            var serviceHistory = _mapper.Map<ServiceHistory>(serviceHistoryDto);
            serviceHistory.Device = device;
            serviceHistory.Responsible = user;

            // Save to repository
            await _serviceHistoryRepository.AddAsync(serviceHistory);

            // Map back to DTO
            return _mapper.Map<ServiceHistoryDto>(serviceHistory);
        }

    }
}
