using MyAspireApp.Domain.Entities;
using MyAspireApp.Domain.ValueObjects;

namespace MyAspireApp.Domain.Tests.Entities;

public class UserTests
{
    [Fact]
    public void ChangeEmail_ShouldUpdateEmail_WhenNewEmailIsValid()
    {
        // Arrange
        var userResult = new UserBuilder()
            .WithId(new UserId(Guid.NewGuid()))
            .WithName("Test User")
            .WithEmail("old@example.com")
            .Build();
        Assert.True(userResult.IsSuccess);
        var user = userResult.Value;
        var newEmail = Email.Create("new@example.com");

        // Act
        user.ChangeEmail(newEmail);

        // Assert
        Assert.Equal(newEmail, user.Email);
    }

    [Fact]
    public void UserBuilder_Build_ShouldReturnSuccess_WhenAllParametersAreValid()
    {
        // Arrange
        var userId = new UserId(Guid.NewGuid());
        var name = "Valid Name";
        var email = "valid@example.com";

        // Act
        var result = new UserBuilder()
            .WithId(userId)
            .WithName(name)
            .WithEmail(email)
            .Build();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(userId, result.Value.Id);
        Assert.Equal(name, result.Value.Name);
        Assert.Equal(email, result.Value.Email.Value);
    }

    [Fact]
    public void UserBuilder_Build_ShouldReturnFailure_WhenNameIsEmpty()
    {
        // Arrange
        var userId = new UserId(Guid.NewGuid());
        var name = "";
        var email = "valid@example.com";

        // Act
        var result = new UserBuilder()
            .WithId(userId)
            .WithName(name)
            .WithEmail(email)
            .Build();

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("UserBuilder.NameEmpty", result.Error.Code);
        Assert.Equal("Name cannot be null or empty.", result.Error.Message);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void UserBuilder_Build_ShouldReturnFailure_WhenEmailIsEmpty(string email)
    {
        // Arrange
        var userId = new UserId(Guid.NewGuid());
        var name = "Valid Name";

        // Act
        var result = new UserBuilder()
            .WithId(userId)
            .WithName(name)
            .WithEmail(email)
            .Build();

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("UserBuilder.InvalidEmail", result.Error.Code);
    }

    [Fact]
    public void UserBuilder_Build_ShouldReturnFailure_WhenEmailHasInvalidFormat()
    {
        // Arrange
        var userId = new UserId(Guid.NewGuid());
        var name = "Valid Name";
        var email = "invalid-email";

        // Act
        var result = new UserBuilder()
            .WithId(userId)
            .WithName(name)
            .WithEmail(email)
            .Build();

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("UserBuilder.InvalidEmail", result.Error.Code);
    }

    [Fact]
    public void UserBuilder_Build_ShouldGenerateId_WhenIdIsNotProvided()
    {
        // Arrange
        var name = "Test User";
        var email = "test@example.com";

        // Act
        var result = new UserBuilder()
            .WithName(name)
            .WithEmail(email)
            .Build();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.NotNull(result.Value.Id);
        Assert.NotEqual(Guid.Empty, result.Value.Id.Value);
        Assert.Equal(name, result.Value.Name);
        Assert.Equal(email, result.Value.Email.Value);
    }
}
