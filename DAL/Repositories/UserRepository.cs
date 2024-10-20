using DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllAsync(int pageNumber, int pageSize);
        Task<User> GetByIdAsync(int id);
    }

    public class UserRepository : IUserRepository
    {
        private readonly MonitoringContext _context;

        public UserRepository(MonitoringContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<User>> GetAllAsync(int pageNumber, int pageSize)
        {
            return await _context.Users
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<User> GetByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }
    }

}
