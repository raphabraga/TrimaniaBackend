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
            return _applicationContext.Users.FirstOrDefault(user => user.Id == id);
        }

        public User GetUserByLogin(string login)
        {
            return _applicationContext.Users.FirstOrDefault(user => user.Login == login);
        }

        public List<User> Query(string query, string sort, int? queryPage)
        {
            int perPage = 10;
            List<User> users;
            if (!string.IsNullOrEmpty(query))
                users = _applicationContext.Users.Include(user => user.Address).Where(user => user.Login.Contains(query) ||
            user.Name.Contains(query) || user.Email.Contains(query)).ToList();
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

        public User UpdateUser(int id)
    }
}