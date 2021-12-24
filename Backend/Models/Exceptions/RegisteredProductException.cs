using System;

namespace Backend.Models.Exceptions
{
    public class RegisteredProductException : Exception
    {
        public RegisteredProductException(string message) : base(message) { }
    }
}