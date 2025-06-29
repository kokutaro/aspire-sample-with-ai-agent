using MyAspireApp.Domain.Entities;

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
        var newEmail = "new@example.com";

        // Act
        var result = user.ChangeEmail(newEmail);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(result.Value);
        Assert.Equal(newEmail, user.Email);
    }

    [Fact]
    public void ChangeEmail_ShouldReturnFailure_WhenNewEmailIsEmpty()
    {
        // Arrange
        var userResult = new UserBuilder()
            .WithId(new UserId(Guid.NewGuid()))
            .WithName("Test User")
            .WithEmail("old@example.com")
            .Build();
        Assert.True(userResult.IsSuccess);
        var user = userResult.Value;
        var newEmail = "";

        // Act
        var result = user.ChangeEmail(newEmail);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("User.EmailEmpty", result.Error.Code);
        Assert.Equal("Email cannot be empty.", result.Error.Message);
        Assert.Equal("old@example.com", user.Email); // Email should not change
    }

    [Fact]
    public void ChangeEmail_ShouldReturnFailure_WhenNewEmailIsWhitespace()
    {
        // Arrange
        var userResult = new UserBuilder()
            .WithId(new UserId(Guid.NewGuid()))
            .WithName("Test User")
            .WithEmail("old@example.com")
            .Build();
        Assert.True(userResult.IsSuccess);
        var user = userResult.Value;
        var newEmail = "   ";

        // Act
        var result = user.ChangeEmail(newEmail);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("User.EmailEmpty", result.Error.Code);
        Assert.Equal("Email cannot be empty.", result.Error.Message);
        Assert.Equal("old@example.com", user.Email); // Email should not change
    }

    [Fact]
    public void ChangeEmail_ShouldReturnFailure_WhenNewEmailIsInvalidFormat()
    {
        // Arrange
        var userResult = new UserBuilder()
            .WithId(new UserId(Guid.NewGuid()))
            .WithName("Test User")
            .WithEmail("old@example.com")
            .Build();
        Assert.True(userResult.IsSuccess);
        var user = userResult.Value;
        var newEmail = "invalid-email"; // Missing @

        // Act
        var result = user.ChangeEmail(newEmail);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("User.InvalidEmailFormat", result.Error.Code);
        Assert.Equal("Invalid email format.", result.Error.Message);
        Assert.Equal("old@example.com", user.Email); // Email should not change
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
        Assert.Equal(email, result.Value.Email);
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

    [Fact]
    public void UserBuilder_Build_ShouldReturnFailure_WhenNameIsWhitespace()
    {
        // Arrange
        var userId = new UserId(Guid.NewGuid());
        var name = "   ";
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

    [Fact]
    public void UserBuilder_Build_ShouldReturnFailure_WhenEmailIsEmpty()
    {
        // Arrange
        var userId = new UserId(Guid.NewGuid());
        var name = "Valid Name";
        var email = "";

        // Act
        var result = new UserBuilder()
            .WithId(userId)
            .WithName(name)
            .WithEmail(email)
            .Build();

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("UserBuilder.EmailEmpty", result.Error.Code);
        Assert.Equal("Email cannot be null or empty.", result.Error.Message);
    }

    [Fact]
    public void UserBuilder_Build_ShouldReturnFailure_WhenEmailIsWhitespace()
    {
        // Arrange
        var userId = new UserId(Guid.NewGuid());
        var name = "Valid Name";
        var email = "   ";

        // Act
        var result = new UserBuilder()
            .WithId(userId)
            .WithName(name)
            .WithEmail(email)
            .Build();

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("UserBuilder.EmailEmpty", result.Error.Code);
        Assert.Equal("Email cannot be null or empty.", result.Error.Message);
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
        Assert.Equal("UserBuilder.InvalidEmailFormat", result.Error.Code);
        Assert.Equal("Invalid email format.", result.Error.Message);
    }
}