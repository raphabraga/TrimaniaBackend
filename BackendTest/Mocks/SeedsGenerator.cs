using System;
using System.Collections.Generic;
using System.Linq;
using Backend.Models;
using Backend.Models.Enums;
using Backend.Utils;
using Microsoft.EntityFrameworkCore;
using BC = BCrypt.Net.BCrypt;

namespace BackendTest.Mocks
{
    public class SeedsGenerator
    {
        private readonly ApplicationContextMoq _context;
        public SeedsGenerator(ApplicationContextMoq context)
        {
            _context = context;
        }
        public Address GenerateAddress()
        {
            return new Address
            {
                Id = _context.Addresses.Count() + 1,
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
            int id = _context.Users.Count() + 1;
            string name = GeneratorUtils.GenerateAttribute("Names", id - 1);
            string[] names = name.Split(" ");
            string login = names[0].Substring(0, 1).ToLower() + "." + names[1].ToLower();
            string email = name.Substring(1, 5) + GeneratorUtils.GenerateNumber(3) + "@mail.com";

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
                AddressId = _context.Addresses.Count()
            };
        }

        public Product GenerateProduct()
        {
            int id = _context.Products.Count() + 1;
            return new Product
            {
                Id = id,
                Name = GeneratorUtils.GenerateAttribute("Products", id - 1),
                Description = GeneratorUtils.GenerateLoremIpsum(10, 20, 3, 5),
                Price = (decimal)(990 * new Random().NextDouble()) + 10,
                StockQuantity = new Random().Next(1, 100)
            };
        }

        public ChartItem GenerateChartItem(int id)
        {
            var rand = new Random();
            var productId = rand.Next(1, _context.Products.AsNoTracking().Count() + 1);
            var clientId = rand.Next(2, _context.Users.AsNoTracking().Count() + 1);
            return new ChartItem
            {
                Id = id,
                ProductId = productId,
                Price = _context.Products.AsNoTracking().FirstOrDefault(product => product.Id == productId).Price.GetValueOrDefault(),
                Quantity = rand.Next(1, 4),
            };
        }
        public Order GenerateOrder()
        {
            var rand = new Random();
            int id = _context.Orders.AsNoTracking().Count() + 1;
            DateTime creationDate = DateTime.Now.AddYears(-rand.Next(1, 3)).AddDays(rand.Next(1, 365)).AddHours(rand.Next(1, 24)).AddMinutes(rand.Next(1, 60));
            OrderStatus status = rand.NextDouble() > 0.5f ? OrderStatus.Finished : OrderStatus.Cancelled;
            DateTime? cancellationDate = status == OrderStatus.Cancelled ?
            creationDate.AddDays(rand.Next(0, 7)).AddHours(rand.Next(1, 24)).AddMinutes(rand.Next(1, 60)) : null;
            DateTime? finishingDate = status == OrderStatus.Finished ?
            creationDate.AddDays(rand.Next(0, 7)).AddHours(rand.Next(1, 24)).AddMinutes(rand.Next(1, 60)) : null;
            var items = new List<ChartItem>();
            int itemId = _context.Items.Count() + 1;
            for (int i = 0; i < rand.Next(1, 6); i++)
            {
                var item = GenerateChartItem(itemId + i);
                items.Add(item);
            }
            decimal totalValue = items.Sum(item => item.Price * item.Quantity);
            return new Order
            {
                Id = id,
                ClientId = rand.Next(2, _context.Users.AsNoTracking().Count() + 1),
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