using MyAspireApp.Domain.Common;
using Xunit;

namespace MyAspireApp.Domain.Tests.Common;

public class ResultTests
{
    private static readonly Error TestError = new("TestCode", "TestMessage");

    [Fact]
    public void Success_ShouldCreateSuccessfulResult()
    {
        // Arrange
        var value = "TestValue";

        // Act
        var result = Result<string>.Success(value);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.False(result.IsFailure);
        Assert.Equal(value, result.Value);
    }

    [Fact]
    public void Failure_ShouldCreateFailedResult()
    {
        // Act
        var result = Result<string>.Failure(TestError);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.True(result.IsFailure);
        Assert.Equal(TestError, result.Error);
    }

    [Fact]
    public void Value_ShouldReturnCorrectValue_WhenSuccess()
    {
        // Arrange
        var value = "TestValue";
        var result = Result<string>.Success(value);

        // Act
        var actualValue = result.Value;

        // Assert
        Assert.Equal(value, actualValue);
    }

    [Fact]
    public void Value_ShouldThrowInvalidOperationException_WhenFailure()
    {
        // Arrange
        var result = Result<string>.Failure(TestError);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => result.Value);
    }

    [Fact]
    public void Error_ShouldReturnCorrectError_WhenFailure()
    {
        // Arrange
        var result = Result<string>.Failure(TestError);

        // Act
        var actualError = result.Error;

        // Assert
        Assert.Equal(TestError, actualError);
    }

    [Fact]
    public void Error_ShouldThrowInvalidOperationException_WhenSuccess()
    {
        // Arrange
        var value = "TestValue";
        var result = Result<string>.Success(value);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => result.Error);
    }

    [Fact]
    public void ImplicitOperator_FromValue_ShouldCreateSuccessfulResult()
    {
        // Arrange
        string value = "TestValue";

        // Act
        Result<string> result = value;

        // Assert
        Assert.True(result.IsSuccess);
        Assert.False(result.IsFailure);
        Assert.Equal(value, result.Value);
    }

    [Fact]
    public void ImplicitOperator_FromError_ShouldCreateFailedResult()
    {
        // Arrange
        Error error = TestError;

        // Act
        Result<string> result = error;

        // Assert
        Assert.False(result.IsSuccess);
        Assert.True(result.IsFailure);
        Assert.Equal(error, result.Error);
    }
}
