using System.Net.WebSockets;
using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Backend.Data;
using Backend.Models;
using System.Security.Cryptography;
using Backend.Utils;

namespace Backend.Services
{
    public class UserService
    {
        private readonly ApplicationContext _applicationContext;
        private readonly Aes _applicationAes;
        public UserService(ApplicationContext context, Aes aes)
        {
            _applicationAes = aes;
            _applicationContext = context;
            _applicationContext.Database.EnsureCreated();
        }

        public User GetUserById(int id)
        {
            return _applicationContext.Users.Include(user => user.Address).FirstOrDefault(user => user.Id == id);
        }

        public User GetUserByLogin(string login)
        {
            return _applicationContext.Users.FirstOrDefault(user => user.Login == login);
        }

        public List<User> Query(string filter, string sort, int? queryPage)
        {
            int perPage = 10;
            List<User> users;
            if (!string.IsNullOrEmpty(filter))
                users = _applicationContext.Users.Include(user => user.Address).Where(user => user.Login.Contains(filter) ||
            user.Name.Contains(filter) || user.Email.Contains(filter)).ToList();
            else
                users = _applicationContext.Users.Include(user => user.Address).ToList();
            if (sort == "asc")
                users = users.OrderBy(user => user.Name).ToList();
            else if (sort == "desc")
                users = users.OrderByDescending(user => user.Name).ToList();
            int page = queryPage.GetValueOrDefault(1) == 0 ? 1 : queryPage.GetValueOrDefault(1);
            users = users.Skip(perPage * (page - 1)).Take(perPage).ToList();
            return users;
        }

        public User CreateUser(User user)
        {
            user.Password = StringCipher.EncryptString(user.Password, _applicationAes.Key, _applicationAes.IV);
            _applicationContext.Users.Add(user);
            _applicationContext.SaveChanges();
            return user;
        }

        public bool CheckPassword(string login, string pwd)
        {
            User user = GetUserByLogin(login);
            if (user == null)
                return false;
            return StringCipher.DecryptString(user.Password, _applicationAes.Key, _applicationAes.IV) == pwd;
        }

        public bool DeleteUser(int id)
        {
            User user = GetUserById(id);
            if (user == null)
                return false;
            else
            {
                _applicationContext.Users.Remove(user);
                _applicationContext.SaveChanges();
                return true;
            }
        }

        public bool UpdateUser(int id, User userUpdate)
        {
            User user = GetUserById(id);
            if (user == null)
                return false;
            else
            {
                user.Login = userUpdate.Login;
                user.Name = userUpdate.Name;
                user.Cpf = userUpdate.Cpf;
                user.Email = userUpdate.Email;
                user.Password = StringCipher.EncryptString(userUpdate.Password,
                _applicationAes.Key, _applicationAes.IV);
                user.Birthday = userUpdate.Birthday;
                user.Address.State = userUpdate.Address.State;
                user.Address.City = userUpdate.Address.City;
                user.Address.Neighborhood = userUpdate.Address.Neighborhood;
                user.Address.Street = userUpdate.Address.Street;
                user.Address.Number = userUpdate.Address.Number;
                _applicationContext.SaveChanges();
                return true;
            }
        }
    }
}