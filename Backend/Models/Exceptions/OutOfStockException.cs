using System;

namespace Backend.Models.Exceptions
{
    public class OutOfStockException : Exception
    {
        public OutOfStockException(string message) : base(message) { }
    }
}