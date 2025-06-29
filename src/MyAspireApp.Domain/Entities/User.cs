using MyAspireApp.Domain.Common;

namespace MyAspireApp.Domain.Entities;

public sealed record UserId(Guid Id) : StronglyTypedId<Guid>(Id);
public class User : Entity<UserId>
{
    public string? Name { get; private set; }
    public string Email { get; private set; }

    internal User(UserId id, string? name, string? email) : base(id)
    {
        Name = name;
        Email = email;
    }

    public Result<bool> ChangeEmail(string newEmail)
    {
        if (string.IsNullOrWhiteSpace(newEmail))
        {
            return Result<bool>.Failure(new Error("User.EmailEmpty", "Email cannot be empty."));
        }
        if (!IsValidEmail(newEmail)) // 仮のバリデーション
        {
            return Result<bool>.Failure(new Error("User.InvalidEmailFormat", "Invalid email format."));
        }

        Email = newEmail;
        return Result<bool>.Success(true);
    }

    private static bool IsValidEmail(string email) => email.Contains('@'); // 簡易的な例
}
