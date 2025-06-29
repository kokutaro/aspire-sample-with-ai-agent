using MediatR;
using MyAspireApp.Domain.Common;
using MyAspireApp.Domain.Entities;
using MyAspireApp.Domain.Repositories;

namespace MyAspireApp.Application.Users.Commands.CreateUser;

internal sealed class CreateUserCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
    : IRequestHandler<CreateUserCommand, Result<UserResponse>>
{
    public async Task<Result<UserResponse>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        if (!await userRepository.IsEmailUniqueAsync(request.Email, cancellationToken))
        {
            return Result<UserResponse>.Failure(new Error("User.EmailNotUnique", "The email is already in use."));
        }

        var userResult = new UserBuilder()
            .WithName(request.Name)
            .WithEmail(request.Email)
            .Build();

        if (userResult.IsFailure)
        {
            return Result<UserResponse>.Failure(userResult.Error);
        }

        var user = userResult.Value;
        userRepository.Add(user);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<UserResponse>.Success(new UserResponse(user.Id.Value, user.Name, user.Email.Value));
    }
}