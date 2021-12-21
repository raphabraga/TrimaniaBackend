using System;

namespace Backend.Models.ViewModels
{
    public class ViewUser
    {
        public ViewUser(User user)
        {
            if (user != null)
            {
                Id = user.Id;
                Login = user.Login;
                Name = user.Name;
                Cpf = user.Cpf;
                Email = user.Email;
                Birthday = user.Birthday;
                Address = user.Address;
            }
        }
        public int Id { get; set; }
        public string Login { get; set; }
        public string Name { get; set; }
        public string Cpf { get; set; }
        public string Email { get; set; }
        public DateTime? Birthday { get; set; }
        public Address Address { get; set; }
    }
}