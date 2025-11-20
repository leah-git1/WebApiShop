using Entities;
using Repository;

namespace Services
{
    public class UsersServices : IUsersRepository, IUsersServices
    {
        private readonly IUsersRepository _usersRepository;
        private readonly IPasswordService _passwordService;
        
        public UsersServices(IUsersRepository usersRepository, IPasswordService passwordService)
        {
            _usersRepository = usersRepository;
            _passwordService = passwordService;
        }

        public User GetUserById(int id)
        {
            return _usersRepository.GetUserById(id);
        }

        public User RegisterUser(User user)
        {
            CheckPassword checkPassword = _passwordService.CheckStrengthPassword(user.Password);
            if (checkPassword.Strength < 2)
            {
                return null;
            }
            return _usersRepository.RegisterUser(user);
        }
        
        public User LoginUser(UserLog userToLog)
        {
            return _usersRepository.LoginUser(userToLog);
        }
        
        public User UpdateUser(User user, int id)
        {
            CheckPassword checkPassword = _passwordService.CheckStrengthPassword(user.Password);
            if (checkPassword.Strength < 2)
            {
                return null;
            }
            return _usersRepository.UpdateUser(user, id);

        }
    }
}
