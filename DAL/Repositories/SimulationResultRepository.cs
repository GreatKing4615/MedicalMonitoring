using DAL.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public interface ISimulationResultRepository
    {
        Task AddAsync(SimulationResult simulationResult);
        Task<List<SimulationResult>> GetAllAsync();
        Task<List<SimulationResult>> GetByDateAsync(DateTimeOffset simulationDate);
        Task ClearAllAsync();
    }
    public class SimulationResultRepository : ISimulationResultRepository
    {
        private readonly MonitoringContext _context;

        public SimulationResultRepository(MonitoringContext context)
        {
            _context = context;
        }

        public async Task AddAsync(SimulationResult simulationResult)
        {
            await _context.SimulationResults.AddAsync(simulationResult);
            await _context.SaveChangesAsync();
        }

        public async Task<List<SimulationResult>> GetAllAsync()
        {
            return await _context.SimulationResults.Include(sr => sr.Device).ToListAsync();
        }

        public async Task<List<SimulationResult>> GetByDateAsync(DateTimeOffset simulationDate)
        {
            return await _context.SimulationResults
                .Include(sr => sr.Device)
                .Where(sr => sr.SimulationDate.Date == simulationDate.Date)
                .ToListAsync();
        }

        public async Task ClearAllAsync()
        {
            _context.SimulationResults.RemoveRange(_context.SimulationResults);
            await _context.SaveChangesAsync();
        }
    }
}
