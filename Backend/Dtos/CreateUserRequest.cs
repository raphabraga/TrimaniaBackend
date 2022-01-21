using System;
using Backend.Models;

namespace Backend.Dtos
{
    public class CreateUserRequest
    {
        public string Login { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string Cpf { get; set; }
        public string Email { get; set; }
        public DateTime? Birthday { get; set; }
        public Address Address { get; set; }

    }
}