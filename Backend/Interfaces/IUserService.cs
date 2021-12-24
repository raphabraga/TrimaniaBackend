using System.Collections.Generic;
using Backend.Models;
using Backend.Models.ViewModels;

namespace Backend.Interfaces
{
    public interface IUserService
    {
        public abstract User GetUserById(int id);
        public abstract User GetUserByLogin(string login);
        public abstract List<User> Query(string filter, string sort, int? queryPage);
        public abstract User CreateUser(User user);
        public abstract string GetAuthenticationToken(AuthUser user);
        public abstract void DeleteUser(int id);
        public abstract User UpdateUser(int id, UpdateUser userUpdate);

    }
}