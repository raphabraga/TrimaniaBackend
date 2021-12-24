using System;

namespace Backend.Models.Exceptions
{
    public class UsedLoginException : Exception
    {
        public UsedLoginException(string message) : base(message) { }
    }
}