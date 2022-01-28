using System.Collections.Generic;
using System.Threading.Tasks;
using Backend.Dtos;
using Backend.Models;

namespace Backend.Interfaces.Services
{
    public interface IUserService
    {
        public abstract Task<User> GetUserById(User requestingUser, int id);
        public abstract Task<User> GetUserByLogin(string login);
        public abstract Task<List<User>> GetUsers(SearchUserRequest searchRequest);
        public abstract Task<User> CreateUser(User user);
        public abstract Task<string> GetAuthenticationToken(AuthenticationRequest user);
        public abstract Task DeleteUser(User user);
        public abstract Task DeleteUserById(int id);
        public abstract Task<User> UpdateUser(User user, UpdateUserRequest userUpdate);

    }
}