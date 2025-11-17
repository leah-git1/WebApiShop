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

        public User getUserById(int id)
        {
            return _iUsersRepository.getUserById(id);
        }

        public User registerUser(User user)
        {
            CheckPassword checkPassword = _iPasswordService.checkStrengthPassword(user.password);
            if (checkPassword.strength < 2)
            {
                return null;
            }
            return _iUsersRepository.registerUser(user);
        }
        public User loginUser(UserLog userToLog)
        {
            return _iUsersRepository.loginUser(userToLog);
        }
        public User updateUser(User user, int id)
        {
            CheckPassword checkPassword = _iPasswordService.checkStrengthPassword(user.password);
            if (checkPassword.strength < 2)
            {
                return null;
            }
            return _iUsersRepository.updateUser(user, id);

        }
    }
}
