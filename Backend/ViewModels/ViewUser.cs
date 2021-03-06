using System;
using System.Text.Json.Serialization;
using Backend.Models;
using Backend.Serializers;

namespace Backend.ViewModels
{
    public class ViewUser
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string Role { get; set; }
        public string Name { get; set; }
        public string Cpf { get; set; }
        public string Email { get; set; }

        [JsonConverter(typeof(DateJsonConverter))]
        public DateTime? Birthday { get; set; }
        public Address Address { get; set; }
        public ViewUser(User user)
        {
            if (user != null)
            {
                Id = user.Id;
                Login = user.Login;
                Role = user.Role;
                Name = user.Name;
                Cpf = user.Cpf;
                Email = user.Email;
                Birthday = user.Birthday;
                Address = user.Address;
            }
        }
    }
}