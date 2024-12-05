using DAL.Entities;
using DAL.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public interface IEquipmentLoadForecastRepository
    {
        Task AddRangeAsync(IEnumerable<EquipmentLoadForecast> forecasts);
        Task<List<EquipmentLoadForecast>> GetForecastsAsync(DeviceType deviceType, DateTimeOffset generatedDate);
        Task ClearOldForecastsAsync(DeviceType deviceType, DateTimeOffset generatedDate);
    }

    public class EquipmentLoadForecastRepository : IEquipmentLoadForecastRepository
    {
        private readonly MonitoringContext _context;

        public EquipmentLoadForecastRepository(MonitoringContext context)
        {
            _context = context;
        }

        public async Task AddRangeAsync(IEnumerable<EquipmentLoadForecast> forecasts)
        {
            await _context.EquipmentLoadForecasts.AddRangeAsync(forecasts);
            await _context.SaveChangesAsync();
        }

        public async Task<List<EquipmentLoadForecast>> GetForecastsAsync(DeviceType deviceType, DateTimeOffset generatedDate)
        {
            return await _context.EquipmentLoadForecasts
                .Where(f => f.DeviceType == deviceType && f.GeneratedDate == generatedDate)
                .OrderBy(f => f.Date)
                .ToListAsync();
        }

        public async Task ClearOldForecastsAsync(DeviceType deviceType, DateTimeOffset generatedDate)
        {
            var oldForecasts = _context.EquipmentLoadForecasts
                .Where(f => f.DeviceType == deviceType && f.GeneratedDate < generatedDate);

            _context.EquipmentLoadForecasts.RemoveRange(oldForecasts);
            await _context.SaveChangesAsync();
        }
    }

}
