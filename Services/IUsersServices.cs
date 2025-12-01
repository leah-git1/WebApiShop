using Entities;
using Repository;
namespace Services
{
    public interface IUsersServices
    {
        Task<User> getUserById(int id);
        Task<User> loginUser(UserLog userToLog);
        Task<User> registerUser(User user);
        Task<User> updateUser(User user, int id);
    }
}