
using MyAspireApp.Domain.Exceptions;
using MyAspireApp.Domain.ValueObjects;

namespace MyAspireApp.Domain.Tests.ValueObjects;

public class EmailTests
{
    [Fact]
    public void Create_WithValidEmail_ShouldReturnEmail()
    {
        // Arrange
        var email = "test@example.com";

        // Act
        var result = Email.Create(email);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(email, result.Value);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Create_WithEmptyEmail_ShouldThrowDomainException(string? email)
    {
        // Act & Assert
        Assert.Throws<DomainException>(() => Email.Create(email));
    }

    [Theory]
    [InlineData("invalid-email")]
    [InlineData("invalid@email")]
    [InlineData("invalid@.com")]
    public void Create_WithInvalidEmail_ShouldThrowDomainException(string email)
    {
        // Act & Assert
        Assert.Throws<DomainException>(() => Email.Create(email));
    }

    [Fact]
    public void Equals_WithSameEmail_ShouldReturnTrue()
    {
        // Arrange
        var email1 = Email.Create("test@example.com");
        var email2 = Email.Create("test@example.com");

        // Act
        var result = email1.Equals(email2);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Equals_WithDifferentEmail_ShouldReturnFalse()
    {
        // Arrange
        var email1 = Email.Create("test@example.com");
        var email2 = Email.Create("another@example.com");

        // Act
        var result = email1.Equals(email2);

        // Assert
        Assert.False(result);
    }
}
