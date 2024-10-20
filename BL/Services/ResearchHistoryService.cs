using AutoMapper;
using BL.Dtos;
using DAL.Repositories;

namespace BL.Services
{
    public interface IResearchHistoryService
    {
        Task<IEnumerable<ResearchHistoryDto>> GetResearchHistoryAsync(int pageNumber, int pageSize);
        Task<ResearchHistoryDto> GetResearchHistoryByIdAsync(int id);
    }

    public class ResearchHistoryService : IResearchHistoryService
    {
        private readonly IResearchHistoryRepository _researchHistoryRepository;
        private readonly IMapper _mapper;

        public ResearchHistoryService(IResearchHistoryRepository researchHistoryRepository, IMapper mapper)
        {
            _researchHistoryRepository = researchHistoryRepository;
            _mapper = mapper;
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
    }
}
