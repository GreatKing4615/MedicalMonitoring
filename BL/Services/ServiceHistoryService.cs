using AutoMapper;
using BL.Dtos;
using DAL.Repositories;

namespace BL.Services
{
    public interface IServiceHistoryService
    {
        Task<IEnumerable<ServiceHistoryDto>> GetServiceHistoryAsync(int pageNumber, int pageSize);
        Task<ServiceHistoryDto> GetServiceHistoryByIdAsync(int id);
    }

    public class ServiceHistoryService : IServiceHistoryService
    {
        private readonly IServiceHistoryRepository _serviceHistoryRepository;
        private readonly IMapper _mapper;

        public ServiceHistoryService(IServiceHistoryRepository serviceHistoryRepository, IMapper mapper)
        {
            _serviceHistoryRepository = serviceHistoryRepository;
            _mapper = mapper;
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
    }
}
