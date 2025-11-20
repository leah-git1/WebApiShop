using Entities;

namespace Repository
{
    public interface IUsersRepository
    {
        User GetUserById(int id);
        User LoginUser(UserLog userToLog);
        User RegisterUser(User user);
        User UpdateUser(User userToUpdate, int id);
    }
}