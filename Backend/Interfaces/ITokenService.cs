using Backend.Models;

namespace Backend.Interfaces
{
    public interface ITokenService
    {
        public abstract string GenerateToken(User user);
    }
}