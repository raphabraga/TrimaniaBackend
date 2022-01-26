using System.Linq;
using Backend.Models;
using BC = BCrypt.Net.BCrypt;

namespace BackendTest.Mocks
{
    public static class DbSeedingMock
    {
        public static void Seeding(ApplicationContextMoq context)
        {
            var seeder = new SeedsGenerator(context);
            context.Database.EnsureCreated();

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
                    Password = BC.HashPassword("#tr1l0g0"),
                    Birthday = new System.DateTime(2016, 9, 3),
                    CreationDate = new System.DateTime(2016, 9, 3),
                    AddressId = 1
                };
                context.Users.Add(adminUser);
            }
            context.SaveChanges();

            if (context.Users.Count() <= 1)
            {
                for (int i = 2; i <= 50; i++)
                {
                    var address = seeder.GenerateAddress();
                    context.Addresses.Add(address);
                    context.SaveChanges();
                    var user = seeder.GenerateUser();
                    context.Users.Add(user);
                    context.SaveChanges();
                }
            }
            if (!context.Products.Any())
            {
                for (int i = 1; i <= 100; i++)
                {
                    var product = seeder.GenerateProduct();
                    context.Products.Add(product);
                    context.SaveChanges();
                }
            }
            if (!context.Orders.Any())
            {
                for (int i = 1; i < 200; i++)
                {
                    var order = seeder.GenerateOrder();
                    context.Orders.Add(order);
                    context.SaveChanges();
                }
            }
        }
    }
}
