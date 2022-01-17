using System.Net;
using System;

namespace Backend.Models.ViewModels
{
    public class ErrorMessage
    {
        public DateTime TimeStamp { get; set; }
        public HttpStatusCode Status { get; set; }
        public string Error { get; set; }
        public string Message { get; set; }
        public ErrorMessage(Exception exception, HttpStatusCode code)
        {
            TimeStamp = DateTime.Now;
            Status = code;
            Error = exception.GetType().Name;
            Message = exception.Message;
        }
    }
}