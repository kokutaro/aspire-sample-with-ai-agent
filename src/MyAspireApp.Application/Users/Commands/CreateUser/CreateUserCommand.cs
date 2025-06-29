using MediatR;
using MyAspireApp.Domain.Common;

namespace MyAspireApp.Application.Users.Commands.CreateUser;

public sealed record CreateUserCommand(
    string Name,
    string Email
) : IRequest<Result<UserResponse>>;