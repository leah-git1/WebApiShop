using Entities;

namespace Services
{
    public interface IUsersServices
    {
        User GetUserById(int id);
        User LoginUser(UserLog userToLog);
        User RegisterUser(User user);
        User UpdateUser(User user, int id);
    }
}