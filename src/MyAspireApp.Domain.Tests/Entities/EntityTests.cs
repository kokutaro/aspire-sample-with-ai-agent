using MyAspireApp.Domain.Common;
using MyAspireApp.Domain.Entities;
using Xunit;
using System;
using System.Reflection;

namespace MyAspireApp.Domain.Tests.Entities;

// Helper classes for testing
public record TestEntityId : StronglyTypedId<Guid>
{
    public TestEntityId(Guid value) : base(value) { }
}

public class TestEntity : Entity<TestEntityId>
{
    public TestEntity(TestEntityId id) : base(id) { }

    // Parameterless constructor for ORM/serialization testing
    private TestEntity() : base(new TestEntityId(Guid.NewGuid())) { }
}

public class AnotherTestEntity : Entity<TestEntityId>
{
    public AnotherTestEntity(TestEntityId id) : base(id) { }
}

public class EntityTests
{
    [Fact]
    public void Constructor_ShouldSetIdCorrectly()
    {
        // Arrange
        var id = new TestEntityId(Guid.NewGuid());

        // Act
        var entity = new TestEntity(id);

        // Assert
        Assert.Equal(id, entity.Id);
    }

    [Fact]
    public void PrivateConstructor_ShouldInitializeId()
    {
        // Arrange
        var constructor = typeof(TestEntity).GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, Array.Empty<Type>());
        Assert.NotNull(constructor);

        // Act
        var entity = (TestEntity)constructor.Invoke(null);

        // Assert
        Assert.NotNull(entity);
        Assert.NotNull(entity.Id);
        Assert.NotEqual(Guid.Empty, entity.Id.Value);
    }

    [Fact]
    public void Equals_ShouldReturnTrue_WhenIdsAndTypesAreSame()
    {
        // Arrange
        var id = new TestEntityId(Guid.NewGuid());
        var entity1 = new TestEntity(id);
        var entity2 = new TestEntity(id);

        // Act & Assert
        Assert.True(entity1.Equals(entity2));
    }

    [Fact]
    public void Equals_ShouldReturnFalse_WhenIdsAreDifferent()
    {
        // Arrange
        var id1 = new TestEntityId(Guid.NewGuid());
        var id2 = new TestEntityId(Guid.NewGuid());
        var entity1 = new TestEntity(id1);
        var entity2 = new TestEntity(id2);

        // Act & Assert
        Assert.False(entity1.Equals(entity2));
    }

    [Fact]
    public void Equals_ShouldReturnFalse_WhenTypesAreDifferent()
    {
        // Arrange
        var id = new TestEntityId(Guid.NewGuid());
        var entity1 = new TestEntity(id);
        var entity2 = new AnotherTestEntity(id);

        // Act & Assert
        Assert.False(entity1.Equals(entity2));
    }

    [Fact]
    public void Equals_ShouldReturnFalse_WhenObjIsNull()
    {
        // Arrange
        var id = new TestEntityId(Guid.NewGuid());
        var entity = new TestEntity(id);

        // Act & Assert
        Assert.False(entity.Equals(null));
    }

    [Fact]
    public void Equals_ShouldReturnTrue_WhenReferencesAreSame()
    {
        // Arrange
        var id = new TestEntityId(Guid.NewGuid());
        var entity = new TestEntity(id);

        // Act & Assert
        Assert.True(entity.Equals(entity));
    }

    [Fact]
    public void GetHashCode_ShouldBeConsistentWithEquals()
    {
        // Arrange
        var id = new TestEntityId(Guid.NewGuid());
        var entity1 = new TestEntity(id);
        var entity2 = new TestEntity(id);

        // Act & Assert
        Assert.Equal(entity1.GetHashCode(), entity2.GetHashCode());
    }

    [Fact]
    public void GetHashCode_ShouldBeDifferent_WhenIdsAreDifferent()
    {
        // Arrange
        var id1 = new TestEntityId(Guid.NewGuid());
        var id2 = new TestEntityId(Guid.NewGuid());
        var entity1 = new TestEntity(id1);
        var entity2 = new TestEntity(id2);

        // Act & Assert
        Assert.NotEqual(entity1.GetHashCode(), entity2.GetHashCode());
    }

    [Fact]
    public void OperatorEquality_ShouldReturnTrue_WhenBothAreNull()
    {
        // Arrange
        TestEntity? entity1 = null;
        TestEntity? entity2 = null;

        // Act & Assert
        Assert.True(entity1 == entity2);
    }

    [Fact]
    public void OperatorEquality_ShouldReturnFalse_WhenOneIsNull()
    {
        // Arrange
        var id = new TestEntityId(Guid.NewGuid());
        var entity1 = new TestEntity(id);
        TestEntity? entity2 = null;

        // Act & Assert
        Assert.False(entity1 == entity2);
        Assert.False(entity2 == entity1);
    }

    [Fact]
    public void OperatorEquality_ShouldReturnTrue_WhenIdsAndTypesAreSame()
    {
        // Arrange
        var id = new TestEntityId(Guid.NewGuid());
        var entity1 = new TestEntity(id);
        var entity2 = new TestEntity(id);

        // Act & Assert
        Assert.True(entity1 == entity2);
    }

    [Fact]
    public void OperatorEquality_ShouldReturnFalse_WhenIdsAreDifferent()
    {
        // Arrange
        var id1 = new TestEntityId(Guid.NewGuid());
        var id2 = new TestEntityId(Guid.NewGuid());
        var entity1 = new TestEntity(id1);
        var entity2 = new TestEntity(id2);

        // Act & Assert
        Assert.False(entity1 == entity2);
    }

    [Fact]
    public void OperatorInequality_ShouldReturnFalse_WhenBothAreNull()
    {
        // Arrange
        TestEntity? entity1 = null;
        TestEntity? entity2 = null;

        // Act & Assert
        Assert.False(entity1 != entity2);
    }

    [Fact]
    public void OperatorInequality_ShouldReturnTrue_WhenOneIsNull()
    {
        // Arrange
        var id = new TestEntityId(Guid.NewGuid());
        var entity1 = new TestEntity(id);
        TestEntity? entity2 = null;

        // Act & Assert
        Assert.True(entity1 != entity2);
        Assert.True(entity2 != entity1);
    }

    [Fact]
    public void OperatorInequality_ShouldReturnFalse_WhenIdsAndTypesAreSame()
    {
        // Arrange
        var id = new TestEntityId(Guid.NewGuid());
        var entity1 = new TestEntity(id);
        var entity2 = new TestEntity(id);

        // Act & Assert
        Assert.False(entity1 != entity2);
    }

    [Fact]
    public void OperatorInequality_ShouldReturnTrue_WhenIdsAreDifferent()
    {
        // Arrange
        var id1 = new TestEntityId(Guid.NewGuid());
        var id2 = new TestEntityId(Guid.NewGuid());
        var entity1 = new TestEntity(id1);
        var entity2 = new TestEntity(id2);

        // Act & Assert
        Assert.True(entity1 != entity2);
    }
}