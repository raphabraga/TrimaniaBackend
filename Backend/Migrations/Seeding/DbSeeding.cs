using System;
using System.Collections.Generic;
using System.Linq;
using Backend.Data;
using Backend.Migrations.Seeding.Interfaces;
using Backend.Models;
using Backend.Models.Enums;
using Backend.Utils;
using Microsoft.EntityFrameworkCore;
using BC = BCrypt.Net.BCrypt;

namespace Backend.Migrations.Seeding
{
    public class DbSeeding : IDbSeeding
    {
        private readonly ApplicationContext _applicationContext;
        public DbSeeding(ApplicationContext context)
        {
            _applicationContext = context;
        }
        public Address GenerateAddress()
        {
            return new Address
            {
                Id = _applicationContext.Addresses.Count() + 1,
                Number = GeneratorUtils.GenerateNumber(1, 3),
                Street = GeneratorUtils.GenerateAttribute("Streets"),
                Neighborhood = GeneratorUtils.GenerateAttribute("Neighbourhoods"),
                City = GeneratorUtils.GenerateAttribute("Cities"),
                State = GeneratorUtils.GenerateAttribute("States")
            };
        }

        public User GenerateUser()
        {
            var rand = new Random();
            int id = _applicationContext.Users.Count() + 1;
            string name = GeneratorUtils.GenerateAttribute("Names", id - 1);
            string[] names = name.Split(" ");
            string login = names[0].Substring(0, 1).ToLower() + "." + names[1].ToLower();
            string email = login + "@mail.com";

            return new User
            {
                Id = id,
                Name = name,
                Login = login,
                Password = BC.HashPassword(GeneratorUtils.GeneratePassword()),
                Cpf = GeneratorUtils.GenerateNumber(11),
                Email = email,
                Birthday = DateTime.Now.AddYears(-rand.Next(20, 30)).AddDays(rand.Next(1, 365)).AddHours(rand.Next(1, 24)).AddMinutes(rand.Next(1, 60)),
                CreationDate = DateTime.Now.AddYears(-rand.Next(1, 3)).AddDays(rand.Next(1, 365)).AddHours(rand.Next(1, 24)).AddMinutes(rand.Next(1, 60)),
                AddressId = _applicationContext.Addresses.Count()
            };
        }

        public Product GenerateProduct()
        {
            int id = _applicationContext.Products.Count() + 1;
            return new Product
            {
                Id = id,
                Name = GeneratorUtils.GenerateAttribute("Products", id - 1),
                Description = GeneratorUtils.GenerateLoremIpsum(10, 20, 3, 5),
                Price = (decimal)(990 * new Random().NextDouble()) + 10,
                StockQuantity = new Random().Next(1, 100)
            };
        }

        public CartItem GenerateCartItem(int id)
        {
            var rand = new Random();
            var productId = rand.Next(1, _applicationContext.Products.AsNoTracking().Count() + 1);
            var clientId = rand.Next(2, _applicationContext.Users.AsNoTracking().Count() + 1);
            return new CartItem
            {
                Id = id,
                ProductId = productId,
                Price = _applicationContext.Products.AsNoTracking().FirstOrDefault(product => product.Id == productId).Price.GetValueOrDefault(),
                Quantity = rand.Next(1, 4),
            };
        }
        public Order GenerateOrder()
        {
            var rand = new Random();
            int id = _applicationContext.Orders.AsNoTracking().Count() + 1;
            DateTime creationDate = DateTime.Now.AddYears(-rand.Next(1, 3)).AddDays(rand.Next(1, 365)).AddHours(rand.Next(1, 24)).AddMinutes(rand.Next(1, 60));
            OrderStatus status = rand.NextDouble() > 0.5f ? OrderStatus.Finished : OrderStatus.Cancelled;
            DateTime? cancellationDate = status == OrderStatus.Cancelled ?
            creationDate.AddDays(rand.Next(0, 7)).AddHours(rand.Next(1, 24)).AddMinutes(rand.Next(1, 60)) : null;
            DateTime? finishingDate = status == OrderStatus.Finished ?
            creationDate.AddDays(rand.Next(0, 7)).AddHours(rand.Next(1, 24)).AddMinutes(rand.Next(1, 60)) : null;
            var items = new List<CartItem>();
            int itemId = _applicationContext.Items.Count() + 1;
            for (int i = 0; i < rand.Next(1, 6); i++)
            {
                var item = GenerateCartItem(itemId + i);
                items.Add(item);
            }
            decimal totalValue = items.Sum(item => item.Price * item.Quantity);
            return new Order
            {
                Id = id,
                ClientId = rand.Next(2, _applicationContext.Users.AsNoTracking().Count() + 1),
                CreationDate = creationDate,
                FinishingDate = finishingDate,
                CancellationDate = cancellationDate,
                Status = status,
                Items = items,
                TotalValue = totalValue
            };
        }

    }
}