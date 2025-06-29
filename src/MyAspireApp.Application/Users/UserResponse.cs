namespace MyAspireApp.Application.Users;

public sealed record UserResponse(
    Guid Id,
    string Name,
    string Email
);