using System;

namespace Backend.Models.Exceptions
{
    public class RegisterNotFoundException : Exception
    {
        public RegisterNotFoundException(string message) : base(message) { }
    }
}