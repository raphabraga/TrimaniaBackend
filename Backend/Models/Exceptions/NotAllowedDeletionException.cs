using System;

namespace Backend.Models.Exceptions
{
    public class NotAllowedDeletionException : Exception
    {
        public NotAllowedDeletionException(string message) : base(message) { }
    }
}