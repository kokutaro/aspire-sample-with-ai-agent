using MyAspireApp.Domain.Common;
using MyAspireApp.Domain.ValueObjects;

namespace MyAspireApp.Domain.Entities;

public class UserBuilder
{
    private UserId? _id;
    private string? _name;
    private string? _email;

    public UserBuilder WithId(UserId id)
    {
        _id = id;
        return this;
    }

    public UserBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public UserBuilder WithEmail(string email)
    {
        _email = email;
        return this;
    }

    public Result<User> Build()
    {
        if (_id == null)
        {
            _id = new UserId(Guid.NewGuid());
        }

        if (string.IsNullOrWhiteSpace(_name))
        {
            return Result<User>.Failure(new Error("UserBuilder.NameEmpty", "Name cannot be null or empty."));
        }

        try
        {
            var emailObject = Email.Create(_email);
            return Result<User>.Success(new User(_id, _name, emailObject));
        }
        catch (Exception ex)
        {
            return Result<User>.Failure(new Error("UserBuilder.InvalidEmail", ex.Message));
        }
    }
}
