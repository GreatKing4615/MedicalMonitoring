using DAL.Entities;

namespace DAL.Repositories
{
    public interface IResearchRepository
    {
        Task AddAsync(Research research);
        Task<Research> GetByIdAsync(int id);
    }

    public class ResearchRepository : IResearchRepository
    {
        private readonly MonitoringContext _context;

        public ResearchRepository(MonitoringContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Research research)
        {
            await _context.Researches.AddAsync(research);
            await _context.SaveChangesAsync();
        }

        public async Task<Research> GetByIdAsync(int id)
        {
            return await _context.Researches.FindAsync(id);
        }
    }
}
