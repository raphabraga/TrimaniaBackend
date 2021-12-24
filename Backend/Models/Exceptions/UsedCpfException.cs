using System;

namespace Backend.Models.Exceptions
{
    public class UsedCpfException : Exception
    {
        public UsedCpfException(string message) : base(message) { }
    }
}