using DAL.Entities;
using DAL.Enums;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories
{
    public interface IDeviceRepository
    {
        Task<IEnumerable<Device>> GetDevicesAsync(int pageNumber, int pageSize);
        Task<Device> GetDeviceByIdAsync(int id);
        Task AddDeviceAsync(Device device);
        Task<List<Device>> GetAllDevicesAsync();
        Task<List<Device>> GetDevicesByTypeAsync(DeviceType deviceType);
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
        public async Task AddDeviceAsync(Device device)
        {
            await _context.Devices.AddAsync(device);
            await _context.SaveChangesAsync();
        }
        public async Task<List<Device>> GetAllDevicesAsync()
        {
            return await _context.Devices.ToListAsync();
        }
        public async Task<List<Device>> GetDevicesByTypeAsync(DeviceType deviceType)
        {
            return await _context.Devices
                .Where(d => d.Type == deviceType)
                .ToListAsync();
        }
    }

}
