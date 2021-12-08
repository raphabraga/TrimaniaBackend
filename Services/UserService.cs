using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using TrimaniaBackend.Data;
using TrimaniaBackend.Models;

namespace TrimaniaBackend.Services
{
    public class UserService
    {
        private readonly ApplicationContext _applicationContext;

        public UserService(ApplicationContext context)
        {
            _applicationContext = context;
            _applicationContext.Database.EnsureCreated();
        }

        public List<User> GetUsers()
        {
            return _applicationContext.Users.Include(user => user.Address).ToList();
        }

        public User CreateUser(User user)
        {
            _applicationContext.Users.Add(user);
            _applicationContext.SaveChanges();
            return user;
        }
    }
}