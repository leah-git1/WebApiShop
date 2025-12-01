using Entities;

namespace Repository
{
    public interface IUsersRepository
    {
        Task<User> getUserById(int ind);
        Task<User> loginUser(UserLog userToLog);
        Task<User> registerUser(User user);
        Task<User> updateUser(User userToUpdate, int id);
    }
}