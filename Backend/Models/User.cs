using System;
using System.ComponentModel.DataAnnotations;

namespace Backend.Models
{
    public class User
    {
        private User() { }
        public User(int id, string login, string name, string password, string cpf, string email, DateTime birthday, Address address)
        {
            this.Id = id;
            this.Login = login;
            this.Name = name;
            this.Password = password;
            this.Cpf = cpf;
            this.Email = email;
            this.Birthday = birthday;
            this.Address = address;
            CreationDate = DateTime.Now;
        }
        public int Id { get; set; }
        public string Login { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string Cpf { get; set; }
        public string Email { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime Birthday { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime CreationDate { get; set; }
        public Address Address { get; set; }

        public override string ToString()
        {
            return $"Login: {Login}\nName: {Name}\nCPF: {Cpf}\nEmail: {Email}\nBirthday: {Birthday}\nAddress: {Address}";
        }
    }
}