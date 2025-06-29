using MediatR;
using MyAspireApp.Application;
using MyAspireApp.Application.Users.Commands.CreateUser;
using MyAspireApp.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddOpenApi();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapPost("/users", async (CreateUserCommand command, ISender sender) =>
{
    var result = await sender.Send(command);
    return result.IsSuccess ? Results.Ok(result.Value) : Results.BadRequest(result.Error);
});

app.Run();
