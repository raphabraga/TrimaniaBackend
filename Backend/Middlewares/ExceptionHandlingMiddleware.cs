using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Backend.Models.Exceptions;
using Backend.ViewModels;
using Microsoft.AspNetCore.Http;

namespace Backend.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception e)
            {
                await HandleExceptionAsync(context, e);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var code = HttpStatusCode.InternalServerError;
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            };
            string result;
            switch (exception)
            {
                case InvalidOperationException:
                    code = HttpStatusCode.BadGateway;
                    break;
                case UnauthorizedAccessException:
                    code = HttpStatusCode.Unauthorized;
                    break;
                case RegisterNotFoundException:
                    code = HttpStatusCode.NotFound;
                    break;
                case OutOfStockException:
                    code = HttpStatusCode.UnprocessableEntity;
                    break;
                case RegisteredProductException:
                    code = HttpStatusCode.UnprocessableEntity;
                    break;
                case NotAllowedDeletionException:
                    code = HttpStatusCode.UnprocessableEntity;
                    break;
                case AggregateException:
                    code = HttpStatusCode.UnprocessableEntity;
                    break;
                default:
                    code = HttpStatusCode.InternalServerError;
                    break;
            }
            result = JsonSerializer.Serialize(new ErrorMessage(exception, code), options);
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;
            return context.Response.WriteAsync(result);
        }
    }
}