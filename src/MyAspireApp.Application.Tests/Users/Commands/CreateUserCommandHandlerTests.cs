
using FluentAssertions;
using MyAspireApp.Application.Users.Commands.CreateUser;
using MyAspireApp.Domain.Entities;
using MyAspireApp.Domain.Repositories;
using NSubstitute;

namespace MyAspireApp.Application.Tests.Users.Commands;

public class CreateUserCommandHandlerTests
{
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly CreateUserCommandHandler _handler;

    public CreateUserCommandHandlerTests()
    {
        _handler = new CreateUserCommandHandler(_userRepository, _unitOfWork);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccessResult_WhenEmailIsUnique()
    {
        // Arrange
        var command = new CreateUserCommand("Test User", "test@example.com");
        _userRepository.IsEmailUniqueAsync(command.Email).Returns(true);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Name.Should().Be(command.Name);
        result.Value.Email.Should().Be(command.Email);
        _userRepository.Received(1).Add(Arg.Any<User>());
        await _unitOfWork.Received(1).SaveChangesAsync();
    }

    [Fact]
    public async Task Handle_Should_ReturnFailureResult_WhenEmailIsNotUnique()
    {
        // Arrange
        var command = new CreateUserCommand("Test User", "test@example.com");
        _userRepository.IsEmailUniqueAsync(command.Email).Returns(false);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("User.EmailNotUnique");
        _userRepository.DidNotReceive().Add(Arg.Any<User>());
        await _unitOfWork.DidNotReceive().SaveChangesAsync();
    }

    [Fact]
    public async Task Handle_Should_ReturnFailureResult_WhenNameIsEmpty()
    {
        // Arrange
        var command = new CreateUserCommand("", "test@example.com");
        _userRepository.IsEmailUniqueAsync(command.Email).Returns(true);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("UserBuilder.NameEmpty");
        _userRepository.DidNotReceive().Add(Arg.Any<User>());
        await _unitOfWork.DidNotReceive().SaveChangesAsync();
    }
}
