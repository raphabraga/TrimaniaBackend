using System;
using Backend.Models;
using BC = BCrypt.Net.BCrypt;

namespace Backend.Utils
{
    public static class SeedingUtils
    {
        public static Address GenerateAddress(int id)
        {
            return new Address
            {
                Id = id,
                Number = GeneratorUtils.GenerateNumber(1, 3),
                Street = GeneratorUtils.GenerateAttribute("Streets"),
                Neighborhood = GeneratorUtils.GenerateAttribute("Neighbourhoods"),
                City = GeneratorUtils.GenerateAttribute("Cities"),
                State = GeneratorUtils.GenerateAttribute("States")
            };
        }

        public static User GenerateUser(int id)
        {
            var rand = new Random();
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
                AddressId = id
            };
        }

        public static Product GenerateProduct(int id)
        {
            return new Product
            {
                Id = id,
                Name = GeneratorUtils.GenerateAttribute("Products", id - 1),
                Description = GeneratorUtils.GenerateLoremIpsum(15, 30, 5, 10),
                Price = (decimal)(990 * new Random().NextDouble()) + 10,
                StockQuantity = new Random().Next(1, 100)
            };
        }

        // public static ChartItem GenerateChartItem(int id)
        // {
        //     return new ChartItem
        //     {
        //         Id = id,
        //         Product =
        //     };
        // }

        // public static Order GenerateOrder(int id)
        // {

        // }

    }
}