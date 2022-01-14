using Backend.Models;

namespace Backend.Interfaces.Services
{
    public interface ITokenService
    {
        public abstract string GenerateToken(User user);
    }
}