using DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories
{
    public interface IDeviceRepository
    {
        Task<IEnumerable<Device>> GetDevicesAsync(int pageNumber, int pageSize);
        Task<Device> GetDeviceByIdAsync(int id);
    }

    public class DeviceRepository : IDeviceRepository
    {
        private readonly MonitoringContext _context;

        public DeviceRepository(MonitoringContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Device>> GetDevicesAsync(int pageNumber, int pageSize)
        {
            return await _context.Devices
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<Device> GetDeviceByIdAsync(int id)
        {
            return await _context.Devices.FindAsync(id);
        }
    }

}
