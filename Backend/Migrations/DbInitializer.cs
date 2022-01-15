using System;
using System.Linq;
using Backend.Data;
using Backend.Models;
using Backend.Models.Enums;
using Backend.Utils;
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

        public DbInitializer(IServiceScopeFactory scopeFactory, IConfiguration configuration)
        {
            this._scopeFactory = scopeFactory;
            _configuration = configuration;
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
                            Role = RoleTypes.Admin,
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
                        for (int i = 2; i < 50; i++)
                        {
                            var address = SeedingUtils.GenerateAddress(i);
                            context.Addresses.Add(address);
                            var adminUser = SeedingUtils.GenerateUser(i);
                            context.Users.Add(adminUser);
                        }
                    }
                    if (!context.Products.Any())
                    {
                        for (int i = 1; i <= 100; i++)
                        {
                            var product = SeedingUtils.GenerateProduct(i);
                            context.Products.Add(product);
                        }
                    }
                    context.SaveChanges();
                }
            }
        }
    }
}
