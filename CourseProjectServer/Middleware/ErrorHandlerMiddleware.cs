using CourseProjectServer.Exceptions;
using DataTransferObject;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;
using AccessViolationException = System.AccessViolationException;

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

                    case TestNotFoundException:
                        response.StatusCode = (int)HttpStatusCode.BadRequest;
                        await response.WriteAsync(JsonSerializer.Serialize(new ErrorInfo
                        {
                            Code = 400,
                            Message = "Test with given id not found"
                        }));
                        break;

                    case AccessTokenInvalidException:
                        response.StatusCode = (int)HttpStatusCode.BadRequest;
                        await response.WriteAsync(JsonSerializer.Serialize(new ErrorInfo
                        {
                            Code = 302,
                            Message = "Access token is invalid"
                        }));
                        break;

                    case AccessViolationException:
                        response.StatusCode = (int)HttpStatusCode.BadRequest;
                        await response.WriteAsync(JsonSerializer.Serialize(new ErrorInfo
                        {
                            Code = 401,
                            Message = "Can not access object with given id"
                        }));
                        break;

                    case AttemptAlreadyEndedException:
                        response.StatusCode = (int)HttpStatusCode.BadRequest;
                        await response.WriteAsync(JsonSerializer.Serialize(new ErrorInfo
                        {
                            Code = 402,
                            Message = "Attempt already ended, can not change answers after the end"
                        }));
                        break;

                    case AttemptNotFoundException:
                        response.StatusCode = (int)HttpStatusCode.BadRequest;
                        await response.WriteAsync(JsonSerializer.Serialize(new ErrorInfo
                        {
                            Code = 403,
                            Message = "Can not find attempt with given id"
                        }));
                        break;

                    case CanNotAccessOtherUsersTestException:
                        response.StatusCode = (int)HttpStatusCode.BadRequest;
                        await response.WriteAsync(JsonSerializer.Serialize(new ErrorInfo
                        {
                            Code = 404,
                            Message = "Can not access other user's test"
                        }));
                        break;

                    case CanNotAccessPrivateTestException:
                        response.StatusCode = (int)HttpStatusCode.BadRequest;
                        await response.WriteAsync(JsonSerializer.Serialize(new ErrorInfo
                        {
                            Code = 405,
                            Message = "Can not access private test"
                        }));
                        break;

                    case TooManyAttemptsException:
                        response.StatusCode = (int)HttpStatusCode.BadRequest;
                        await response.WriteAsync(JsonSerializer.Serialize(new ErrorInfo
                        {
                            Code = 406,
                            Message = "Too many attempts"
                        }));
                        break;

                    case UserDoesNotHavePassword:
                        response.StatusCode = (int)HttpStatusCode.BadRequest;
                        await response.WriteAsync(JsonSerializer.Serialize(new ErrorInfo
                        {
                            Code = 303,
                            Message = "User does not have password, can not log in"
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
