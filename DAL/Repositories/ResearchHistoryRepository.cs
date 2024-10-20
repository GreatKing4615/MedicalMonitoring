using DAL.Entities;
using DAL.Enums;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories
{
    public interface IResearchHistoryRepository
    {
        Task<IEnumerable<ResearchHistory>> GetAllAsync(int pageNumber, int pageSize);
        Task<ResearchHistory> GetByIdAsync(int id);
        Task AddAsync(ResearchHistory researchHistory);
        Task<List<PatientFlowData>> GetPatientFlowDataAsync(DeviceType deviceType);

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

        public async Task<List<PatientFlowData>> GetPatientFlowDataAsync(DeviceType deviceType)
        {
            return await _context.ResearchHistories
                .Include(rh => rh.Device)
                .Where(rh => rh.Device.Type == deviceType)
                .GroupBy(rh => rh.ResearchDate.Date)
                .Select(g => new PatientFlowData
                {
                    Date = g.Key,
                    PatientCount = g.Count()
                })
                .OrderBy(data => data.Date)
                .ToListAsync();
        }
    }
}
