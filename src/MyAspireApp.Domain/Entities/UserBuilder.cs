using MyAspireApp.Domain.Common;

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

        if (string.IsNullOrWhiteSpace(_email))
        {
            return Result<User>.Failure(new Error("UserBuilder.EmailEmpty", "Email cannot be null or empty."));
        }

        // 簡易的なメールアドレスバリデーション
        if (!_email.Contains('@'))
        {
            return Result<User>.Failure(new Error("UserBuilder.InvalidEmailFormat", "Invalid email format."));
        }

        return Result<User>.Success(new User(_id, _name, _email));
    }
}
