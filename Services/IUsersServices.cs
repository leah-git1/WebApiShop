using DTOs;
using Entities;
using Repository;
namespace Services
{
    public interface IUsersServices
    {
        Task<UserDTO> getUserById(int id);
        Task<UserDTO> loginUser(UserLog userToLog);
        Task<UserDTO> registerUser(UserToRegisterDTO userToRegister);
        Task<UserDTO> updateUser(UserToRegisterDTO userToUpdate, int id);
    }
}