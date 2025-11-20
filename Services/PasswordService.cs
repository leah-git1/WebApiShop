using Entities;
using Repository;

namespace Services
{
    public class PasswordService : IPasswordService
    {
        public CheckPassword CheckStrengthPassword(string password)
        {
            var result = Zxcvbn.Core.EvaluatePassword(password);
            int strength = result.Score;
            CheckPassword pass = new CheckPassword();
            pass.Password = password;
            pass.Strength = strength;
            return pass;
        }
    }
}
