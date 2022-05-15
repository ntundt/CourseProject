using CourseProjectServer.Exceptions;
using DataTransferObject;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

namespace CourseProjectServer.Middleware
{
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        public ErrorHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                var response = context.Response;
                response.ContentType = "application/json";

                switch (ex)
                {
                    case UserNotFoundException e:
                        response.StatusCode = (int)HttpStatusCode.BadRequest;
                        await response.WriteAsync(JsonSerializer.Serialize(new ErrorInfo
                        {
                            Code = 300,
                            Message = e.Message
                        }));
                        break;
                    case WrongPasswordException:
                        response.StatusCode = (int)HttpStatusCode.BadRequest;
                        await response.WriteAsync(JsonSerializer.Serialize(new ErrorInfo
                        {
                            Code = 301,
                            Message = "Wrong password"
                        }));
                        break;
                    default:
                        response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        await response.WriteAsync(JsonSerializer.Serialize(new ErrorInfo
                        {
                            Code = 1000,
                            Message = "Internal server error. Please try later."
                        }));
                        Console.WriteLine(ex);
                        break;
                }
            }
        }
    }
}
