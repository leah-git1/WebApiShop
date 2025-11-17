using Entities;

namespace Services
{
    public interface IUsersServices
    {
        User getUserById(int id);
        User loginUser(UserLog userToLog);
        User registerUser(User user);
        User updateUser(User user, int id);
    }
}