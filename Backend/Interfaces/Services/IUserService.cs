using System.Collections.Generic;
using Backend.Dtos;
using Backend.Models;

namespace Backend.Interfaces.Services
{
    public interface IUserService
    {
        public abstract User GetUserById(User requestingUser, int id);
        public abstract User GetUserByLogin(string login);
        public abstract List<User> Query(string filter, string sort, int? queryPage);
        public abstract User CreateUser(User user);
        public abstract string GetAuthenticationToken(AuthUser user);
        public abstract void DeleteUser(int id);
        public abstract User UpdateUser(int id, UpdateUser userUpdate);

    }
}