using Entities;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Threading.Tasks;

namespace Repository
{
    public class UsersRepository : IUsersRepository
    {
        private readonly ShopContext _shopContext;
       public UsersRepository(ShopContext shopContext)
        {
            this._shopContext = shopContext;
        }

        public async Task<User> getUserById(int ind)
        {
            return await _ShopContext.Users.FirstOrDefaultAsync(x=>x.UserId==ind);
        }

        public async Task<User> registerUser(User user)
        {
            await _ShopContext.Users.AddAsync(user);
            await _ShopContext.SaveChangesAsync();
            return user;
        }

        public async Task<User> loginUser(UserLog userToLog)
        {
            return await _ShopContext.Users.FirstOrDefaultAsync(x => x.UserName == userToLog.userName && x.Password == userToLog.password);
        }

        public async Task<User> updateUser(User userToUpdate, int id)
        {
             _ShopContext.Users.Update(userToUpdate);
            await _ShopContext.SaveChangesAsync();
            return userToUpdate;
        }
    }
}