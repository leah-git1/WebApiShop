using AutoMapper;
using DTOs;
using Entities;
using Repository;

namespace Services
{
    public class UsersServices : IUsersServices
    {
        private readonly IUsersRepository _usersRepository;
        private readonly IPasswordService _passwordService;
        private readonly IMapper _mapper;

        public UsersServices(IUsersRepository usersRepository, IPasswordService passwordService, IMapper mapper)
        {
            _usersRepository = usersRepository;
            _passwordService = passwordService;
            _mapper = mapper;
        }

        public async Task<UserDTO> getUserById(int id)
        {
            User user = await _usersRepository.getUserById(id);
            return _mapper.Map<User, UserDTO>(user);
        }

        public async Task<UserDTO> registerUser(UserToRegisterDTO userToRegister)
        {
            CheckPassword checkPassword = _passwordService.checkStrengthPassword(userToRegister.Password);
            if (checkPassword.strength < 2)
            {
                return null;
            }
            User user = _mapper.Map<UserToRegisterDTO, User>(userToRegister);
            user = await _usersRepository.registerUser(user);
            return _mapper.Map<User, UserDTO>(user);
        }

        public async Task<UserDTO> loginUser(UserLog userToLog)
        {
            User user = await _usersRepository.loginUser(userToLog);
            return _mapper.Map<User, UserDTO>(user);
        }

        public async Task<UserDTO> updateUser(UserToRegisterDTO userToUpdate, int id)
        {
            CheckPassword checkPassword = _passwordService.checkStrengthPassword(userToUpdate.Password);
            if (checkPassword.strength < 2)
            {
                return null;
            }
            User user = _mapper.Map<UserToRegisterDTO, User>(userToUpdate);
            user.UserId = id;
            user = await _usersRepository.updateUser(user, id);
            return _mapper.Map<User, UserDTO>(user);
        }
    }
}
