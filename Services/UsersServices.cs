using Entities;
using Repository;

namespace Services
{
    public class UsersServices : IUsersRepository, IUsersServices
    {
        IUsersRepository _iUsersRepository;
        IPasswordService _iPasswordService;
        public UsersServices(IUsersRepository iusersRepository, IPasswordService passwordService)
        {
            this._iUsersRepository = iusersRepository;
            this._iPasswordService = passwordService;
        }

        public async Task<User> getUserById(int id)
        {
            return await _iUsersRepository.getUserById(id);
        }

        public async Task<User> registerUser(User user)
        {
            CheckPassword checkPassword = _iPasswordService.checkStrengthPassword(user.Password);
            if (checkPassword.strength < 2)
            {
                return null;
            }
            return await _iUsersRepository.registerUser(user);
        }
        public async Task<User> loginUser(UserLog userToLog)
        {
            return await _iUsersRepository.loginUser(userToLog);
        }
        public async Task<User> updateUser(User user, int id)
        {
            CheckPassword checkPassword = _iPasswordService.checkStrengthPassword(user.Password);
            if (checkPassword.strength < 2)
            {
                return null;
            }
            return await _iUsersRepository.updateUser(user, id);

        }
    }
}
