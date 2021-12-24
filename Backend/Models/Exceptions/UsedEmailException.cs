using System;

namespace Backend.Models.Exceptions
{
    public class UsedEmailException : Exception
    {
        public UsedEmailException(string message) : base(message) { }
    }
}