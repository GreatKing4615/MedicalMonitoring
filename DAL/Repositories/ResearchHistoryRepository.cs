using DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories
{
    public interface IResearchHistoryRepository
    {
        Task<IEnumerable<ResearchHistory>> GetAllAsync(int pageNumber, int pageSize);
        Task<ResearchHistory> GetByIdAsync(int id);
        Task AddAsync(ResearchHistory researchHistory);
    }

    public class ResearchHistoryRepository : IResearchHistoryRepository
    {
        private readonly MonitoringContext _context;

        public ResearchHistoryRepository(MonitoringContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ResearchHistory>> GetAllAsync(int pageNumber, int pageSize)
        {
            return await _context.ResearchHistories
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<ResearchHistory> GetByIdAsync(int id)
        {
            return await _context.ResearchHistories.FindAsync(id);
        }
        public async Task AddAsync(ResearchHistory researchHistory)
        {
            await _context.ResearchHistories.AddAsync(researchHistory);
            await _context.SaveChangesAsync();
        }

    }
}
