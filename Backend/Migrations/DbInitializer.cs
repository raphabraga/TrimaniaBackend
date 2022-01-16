using System;
using System.Linq;
using Backend.Data;
using Backend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using BC = BCrypt.Net.BCrypt;

namespace Backend.Migrations
{
    public class DbInitializer : IDbInitializer
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IConfiguration _configuration;
        private readonly IDbSeeding _dbSeeding;

        public DbInitializer(IServiceScopeFactory scopeFactory, IConfiguration configuration, IDbSeeding dbSeeding)
        {
            _scopeFactory = scopeFactory;
            _configuration = configuration;
            _dbSeeding = dbSeeding;
        }

        public void Initialize()
        {
            using (var serviceScope = _scopeFactory.CreateScope())
            {
                using (var context = serviceScope.ServiceProvider.GetService<ApplicationContext>())
                {
                    context.Database.Migrate();
                }
            }
        }

        public void SeedAdmin()
        {
            using (var serviceScope = _scopeFactory.CreateScope())
            {
                using (var context = serviceScope.ServiceProvider.GetService<ApplicationContext>())
                {
                    try
                    {
                        context.Database.EnsureCreated();
                    }
                    catch (InvalidOperationException e)
                    {
                        System.Console.WriteLine(e.Message);
                    }
                    if (!context.Users.Any())
                    {
                        var adminAddress = new Address
                        {
                            Id = 1,
                            Number = "1305",
                            Street = "Av. Min. Gentil Barreira",
                            Neighborhood = "Sapiranga",
                            City = "Fortaleza",
                            State = "CE"
                        };
                        context.Addresses.Add(adminAddress);
                        var adminUser = new User
                        {
                            Id = 1,
                            Name = "Administrator",
                            Login = "admin",
                            Cpf = "00000000000",
                            Email = "admin@trilogo.com",
                            Role = "Administrator",
                            Password = BC.HashPassword(_configuration.GetValue<string>("AdminPassword")),
                            Birthday = new System.DateTime(2016, 9, 3),
                            CreationDate = new System.DateTime(2016, 9, 3),
                            AddressId = 1
                        };
                        context.Users.Add(adminUser);
                    }
                    context.SaveChanges();
                }
            }
        }
        public void SeedData()
        {
            using (var serviceScope = _scopeFactory.CreateScope())
            {
                using (var context = serviceScope.ServiceProvider.GetService<ApplicationContext>())
                {
                    if (context.Users.Count() <= 1)
                    {
                        for (int i = 2; i <= 50; i++)
                        {
                            var address = _dbSeeding.GenerateAddress();
                            context.Addresses.Add(address);
                            context.SaveChanges();
                            var user = _dbSeeding.GenerateUser();
                            context.Users.Add(user);
                            context.SaveChanges();
                        }
                    }
                    if (!context.Products.Any())
                    {
                        for (int i = 1; i <= 100; i++)
                        {
                            var product = _dbSeeding.GenerateProduct();
                            context.Products.Add(product);
                            context.SaveChanges();
                        }
                    }
                    if (!context.Orders.Any())
                    {
                        for (int i = 1; i < 200; i++)
                        {
                            var order = _dbSeeding.GenerateOrder();
                            context.Orders.Add(order);
                            context.SaveChanges();
                        }
                    }
                }
            }
        }
    }
}
