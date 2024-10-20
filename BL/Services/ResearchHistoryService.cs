using AutoMapper;
using BL.Dtos;
using DAL.Entities;
using DAL.Repositories;
using System.ComponentModel.DataAnnotations;

namespace BL.Services
{
    public interface IResearchHistoryService
    {
        Task<IEnumerable<ResearchHistoryDto>> GetResearchHistoryAsync(int pageNumber, int pageSize);
        Task<ResearchHistoryDto> GetResearchHistoryByIdAsync(int id);
        Task<ResearchHistoryDto> CreateResearchHistoryAsync(ResearchHistoryDto researchHistoryDto);

    }

    public class ResearchHistoryService : IResearchHistoryService
    {
        private readonly IResearchHistoryRepository _researchHistoryRepository;
        private readonly IDeviceRepository _deviceRepository;
        private readonly IResearchRepository _researchRepository;
        private readonly IMapper _mapper;

        public ResearchHistoryService(IResearchHistoryRepository researchHistoryRepository, IMapper mapper, IDeviceRepository deviceRepository, IResearchRepository researchRepository)
        {
            _researchHistoryRepository = researchHistoryRepository;
            _mapper = mapper;
            _deviceRepository = deviceRepository;
            _researchRepository = researchRepository;
        }

        public async Task<IEnumerable<ResearchHistoryDto>> GetResearchHistoryAsync(int pageNumber, int pageSize)
        {
            var history = await _researchHistoryRepository.GetAllAsync(pageNumber, pageSize);
            return _mapper.Map<IEnumerable<ResearchHistoryDto>>(history);
        }

        public async Task<ResearchHistoryDto> GetResearchHistoryByIdAsync(int id)
        {
            var history = await _researchHistoryRepository.GetByIdAsync(id);
            if (history == null)
            {
                throw new KeyNotFoundException("Research history not found");
            }
            return _mapper.Map<ResearchHistoryDto>(history);
        }

        public async Task<ResearchHistoryDto> CreateResearchHistoryAsync(ResearchHistoryDto researchHistoryDto)
        {
            // Business logic: Validate input
            if (researchHistoryDto.ResearchId == 0)
                throw new ValidationException("ResearchId is required.");

            if (researchHistoryDto.DeviceId == 0)
                throw new ValidationException("DeviceId is required.");

            // Check if Research exists
            var research = await _researchRepository.GetByIdAsync(researchHistoryDto.ResearchId);
            if (research == null)
                throw new ValidationException("Research not found.");

            // Check if Device exists
            var device = await _deviceRepository.GetDeviceByIdAsync(researchHistoryDto.DeviceId);
            if (device == null)
                throw new ValidationException("Device not found.");

            // Map DTO to entity
            var researchHistory = _mapper.Map<ResearchHistory>(researchHistoryDto);
            researchHistory.Research = research;
            researchHistory.Device = device;

            // Save to repository
            await _researchHistoryRepository.AddAsync(researchHistory);

            // Map back to DTO
            return _mapper.Map<ResearchHistoryDto>(researchHistory);
        }

    }
}
