using System;

namespace Backend.Models.ViewModels
{
    public class UpdateUser
    {
        public string Name { get; set; }

        public string Password { get; set; }
        public DateTime? Birthday { get; set; }
        public Address Address { get; set; }
    }
}