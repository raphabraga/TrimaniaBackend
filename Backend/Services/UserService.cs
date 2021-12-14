using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Backend.Data;
using Backend.Models;

namespace Backend.Services
{
    public class UserService
    {
        private readonly ApplicationContext _applicationContext;

        public UserService(ApplicationContext context)
        {
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
            _applicationContext.Users.Add(user);
            _applicationContext.SaveChanges();
            return user;
        }

        public bool CheckPassword(string login, string pwd)
        {
            User user = GetUserByLogin(login);
            if (user == null)
                return false;
            return user.Password == pwd;
        }
    }
}