using System;
using Backend.Models;

namespace Backend.Dtos
{
    public class UpdateUserRequest
    {
        public string Name { get; set; }
        public string Password { get; set; }
        public DateTime? Birthday { get; set; }
        public Address Address { get; set; }

    }
}