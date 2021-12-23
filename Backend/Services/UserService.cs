using System.Reflection.PortableExecutable;
using System.Net.WebSockets;
using System;
using BC = BCrypt.Net.BCrypt;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Backend.Data;
using Backend.Models;
using Backend.Models.ViewModels;
using Backend.Interfaces;
using Backend.Utils;
using Backend.Models.Exceptions;

namespace Backend.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationContext _applicationContext;
        public UserService(ApplicationContext context)
        {
            _applicationContext = context;
            try
            {
                _applicationContext.Database.EnsureCreated();
            }
            catch (InvalidOperationException e)
            {
                System.Console.WriteLine(e.Message);
            }
        }

        public User GetUserById(int id)
        {
            try
            {
                return _applicationContext.Users.Include(user => user.Address).FirstOrDefault(user => user.Id == id);
            }
            catch (InvalidOperationException e)
            {
                System.Console.WriteLine(e.Message);
                throw;
            }
        }

        public User GetUserByLogin(string login)
        {
            try
            {
                return _applicationContext.Users.FirstOrDefault(user => user.Login == login);
            }
            catch (InvalidOperationException e)
            {
                System.Console.WriteLine(e.Message);
                throw;
            }
        }

        public List<User> Query(string filter, string sort, int? queryPage)
        {
            int perPage = 10;
            List<User> users;
            try
            {
                users = _applicationContext.Users.Include(user => user.Address).ToList();
                if (!string.IsNullOrEmpty(filter))
                    users = users.Where(user => user.Login.CaseInsensitiveContains(filter) ||
                user.Name.CaseInsensitiveContains(filter) || user.Email.CaseInsensitiveContains(filter)).ToList();
                if (sort == "asc")
                    users = users.OrderBy(user => user.Name).ToList();
                else if (sort == "desc")
                    users = users.OrderByDescending(user => user.Name).ToList();
                int page = queryPage.GetValueOrDefault(1) == 0 ? 1 : queryPage.GetValueOrDefault(1);
                users = users.Skip(perPage * (page - 1)).Take(perPage).ToList();
                return users;
            }
            catch (InvalidOperationException e)
            {
                System.Console.WriteLine(e.Message);
                throw;
            }

        }

        public User CreateUser(User user)
        {
            user.Password = BC.HashPassword(user.Password);
            try
            {
                _applicationContext.Users.Add(user);
                _applicationContext.SaveChanges();
                return user;
            }
            catch (InvalidOperationException e)
            {
                System.Console.WriteLine(e.Message);
                throw;
            }
        }

        public bool CheckPassword(User user, string pwd)
        {
            return BC.Verify(pwd, user.Password);
        }

        public void DeleteUser(int id)
        {
            try
            {
                User user = GetUserById(id);
                bool hasRegisteredOrders = _applicationContext.Orders.Any(order => order.Client.Id == id);
                if (hasRegisteredOrders)
                    throw new NotAllowedDeletionException("User has registered orders, deletion is forbidden");
                else
                {
                    _applicationContext.Users.Remove(user);
                    _applicationContext.SaveChanges();
                }
            }
            catch (InvalidOperationException e)
            {
                System.Console.WriteLine(e.Message);
                throw;
            }
            catch (NotAllowedDeletionException e)
            {
                System.Console.WriteLine(e.Message);
                throw;
            }
        }

        public void UpdateUser(int id, UpdateUser userUpdate)
        {
            try
            {
                User user = GetUserById(id);
                user.Name = userUpdate.Name;
                user.Password = BC.HashPassword(userUpdate.Password);
                user.Birthday = userUpdate.Birthday;
                user.Address.State = userUpdate.Address.State;
                user.Address.City = userUpdate.Address.City;
                user.Address.Neighborhood = userUpdate.Address.Neighborhood;
                user.Address.Street = userUpdate.Address.Street;
                user.Address.Number = userUpdate.Address.Number;
                _applicationContext.SaveChanges();
            }
            catch (InvalidOperationException e)
            {
                System.Console.WriteLine(e.Message);
                throw;
            }
        }
    }
}