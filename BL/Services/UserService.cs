using AutoMapper;
using BL.Dtos;
using DAL.Repositories;

namespace BL.Services
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetUsersAsync(int pageNumber, int pageSize);
        Task<UserDto> GetUserByIdAsync(int id);
    }

    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UserService(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<UserDto>> GetUsersAsync(int pageNumber, int pageSize)
        {
            var users = await _userRepository.GetAllAsync(pageNumber, pageSize);
            return _mapper.Map<IEnumerable<UserDto>>(users);
        }

        public async Task<UserDto> GetUserByIdAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found");
            }
            return _mapper.Map<UserDto>(user);
        }
    }
}
