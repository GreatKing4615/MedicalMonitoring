using AutoMapper;
using BL.Dtos;
using DAL.Entities;
using DAL.Repositories;

namespace BL.Services
{
    public interface IResearchService
    {
        Task<ResearchDto> CreateResearchAsync(ResearchDto researchDto);
        Task<ResearchDto> GetResearchByIdAsync(int id);
    }

    public class ResearchService : IResearchService
    {
        private readonly IResearchRepository _researchRepository;
        private readonly IMapper _mapper;

        public ResearchService(IResearchRepository researchRepository, IMapper mapper)
        {
            _researchRepository = researchRepository;
            _mapper = mapper;
        }

        public async Task<ResearchDto> CreateResearchAsync(ResearchDto researchDto)
        {
            var research = _mapper.Map<Research>(researchDto);
            await _researchRepository.AddAsync(research);
            return _mapper.Map<ResearchDto>(research);
        }

        public async Task<ResearchDto> GetResearchByIdAsync(int id)
        {
            var research = await _researchRepository.GetByIdAsync(id);
            if (research == null)
            {
                throw new KeyNotFoundException("Research not found");
            }
            return _mapper.Map<ResearchDto>(research);
        }
    }

}
