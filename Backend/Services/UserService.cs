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

        public List<User> GetUsers(params object[] obj)
        {
            if (obj.Length < 1)
                return _applicationContext.Users.Include(user => user.Address).ToList();
            else
            {
                User user = _applicationContext.Users.FirstOrDefault(user => user.Id == (int)obj[0]);
                return new List<User>() { user };
            }
        }

        public User GetUserByLogin(string login)
        {
            return _applicationContext.Users.FirstOrDefault(User => User.Login == login);
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
    }
}