using Entities;

namespace Repository
{
    public interface IUsersRepository
    {
        User getUserById(int ind);
        User loginUser(UserLog userToLog);
        User registerUser(User user);
        User updateUser(User userToUpdate, int id);
    }
}