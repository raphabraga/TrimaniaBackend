using System;
using Backend.Dtos;

namespace Backend.Models
{
    public class User
    {
        public User()
        {
            CreationDate = DateTime.Now;
        }

        public User(CreateUserRequest userRequest) : base()
        {
            Id = 0;
            Login = userRequest.Login;
            Name = userRequest.Name;
            Email = userRequest.Email;
            Birthday = userRequest.Birthday;
            Cpf = userRequest.Cpf;
            Password = userRequest.Password;
            Address = userRequest.Address;
            AddressId = 0;
        }
        public int Id { get; set; }
        public string Login { get; set; }
        public string Name { get; set; }
        public string Role { get; set; } = "Customer";
        public string Password { get; set; }
        public string Cpf { get; set; }
        public string Email { get; set; }
        public DateTime? Birthday { get; set; }
        public DateTime? CreationDate { get; set; }
        public Address Address { get; set; }
        public int? AddressId { get; set; }
    }
}