using DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories
{
    public interface IServiceHistoryRepository
    {
        Task<IEnumerable<ServiceHistory>> GetAllAsync(int pageNumber, int pageSize);
        Task<ServiceHistory> GetByIdAsync(int id);
    }

    public class ServiceHistoryRepository : IServiceHistoryRepository
    {
        private readonly MonitoringContext _context;

        public ServiceHistoryRepository(MonitoringContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ServiceHistory>> GetAllAsync(int pageNumber, int pageSize)
        {
            return await _context.ServiceHistories
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<ServiceHistory> GetByIdAsync(int id)
        {
            return await _context.ServiceHistories.FindAsync(id);
        }
    }
}
