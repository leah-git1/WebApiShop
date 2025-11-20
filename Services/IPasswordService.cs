using Entities;

namespace Services
{
    public interface IPasswordService
    {
        CheckPassword CheckStrengthPassword(string password);
    }
}