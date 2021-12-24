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
            catch (InvalidOperationException)
            {
                throw;
            }
        }

        public User GetUserByLogin(string login)
        {
            try
            {
                return _applicationContext.Users.FirstOrDefault(user => user.Login == login);
            }
            catch (InvalidOperationException)
            {
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
            catch (InvalidOperationException)
            {
                throw;
            }

        }

        public User CreateUser(User user)
        {
            user.Password = BC.HashPassword(user.Password);
            try
            {
                var exceptions = new List<Exception>();
                if (_applicationContext.Users.Any(u => u.Login == user.Login))
                    exceptions.Add(new UsedLoginException("User already registered on the database with this login."));
                if (_applicationContext.Users.Any(u => u.Email == user.Email))
                    exceptions.Add(new UsedEmailException("User already registered on the database with this email."));
                if (_applicationContext.Users.Any(u => u.Cpf == user.Cpf))
                    exceptions.Add(new UsedCpfException("User already registered on the database with this CPF."));
                if (exceptions.Count > 0)
                    throw new AggregateException(exceptions);
                _applicationContext.Users.Add(user);
                _applicationContext.SaveChanges();
                return user;
            }
            catch (InvalidOperationException)
            {
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
                    throw new NotAllowedDeletionException("User has registered orders. Deletion is forbidden");
                else
                {
                    _applicationContext.Users.Remove(user);
                    _applicationContext.SaveChanges();
                }
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            catch (NotAllowedDeletionException)
            {
                throw;
            }
        }

        public User UpdateUser(int id, UpdateUser userUpdate)
        {
            try
            {
                User user = GetUserById(id);
                user.Name = string.IsNullOrEmpty(userUpdate.Name) ? user.Name : userUpdate.Name;
                if (userUpdate.Password != null)
                    user.Password = BC.HashPassword(userUpdate.Password);
                user.Birthday = (userUpdate.Birthday == null) ? user.Birthday : userUpdate.Birthday;
                if (user.Address == null)
                {
                    if (userUpdate.Address != null)
                        user.Address = new Address
                        {
                            State = userUpdate?.Address?.State,
                            City = userUpdate?.Address?.City,
                            Neighborhood = userUpdate?.Address?.Neighborhood,
                            Street = userUpdate?.Address?.Street,
                            Number = userUpdate?.Address?.Number
                        };
                }
                else
                {
                    if (userUpdate.Address != null)
                    {
                        user.Address.State = (userUpdate?.Address?.State == null) ? user?.Address?.State : userUpdate?.Address?.State;
                        user.Address.City = (userUpdate?.Address?.City == null) ? user?.Address?.City : userUpdate?.Address?.City;
                        user.Address.Neighborhood = (userUpdate?.Address?.Neighborhood == null) ? user?.Address?.Neighborhood : userUpdate?.Address?.Neighborhood;
                        user.Address.Street = (userUpdate?.Address?.Street == null) ? user?.Address?.Street : userUpdate?.Address?.Street;
                        user.Address.Number = (userUpdate?.Address?.Number == null) ? user?.Address?.Number : userUpdate?.Address?.Number;
                    }
                }
                _applicationContext.SaveChanges();
                return user;
            }
            catch (InvalidOperationException)
            {
                throw;
            }
        }
    }
}