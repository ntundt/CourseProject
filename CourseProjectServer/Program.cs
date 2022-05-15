using CourseProjectServer.Middleware;
using CourseProjectServer;
using System.Text.Json;
using DataTransferObject;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseMiddleware(typeof(ErrorHandlerMiddleware));
//app.UseExceptionHandler(new ErrorHandlerMiddleware())

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

