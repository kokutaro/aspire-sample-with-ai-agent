using MyAspireApp.Domain.Common;
using MyAspireApp.Domain.ValueObjects;

namespace MyAspireApp.Domain.Entities;

public sealed record UserId(Guid Value) : StronglyTypedId<Guid>(Value);
public class User : Entity<UserId>
{
    public string? Name { get; private set; }
    public Email Email { get; private set; }

    internal User(UserId id, string? name, Email email) : base(id)
    {
        Name = name;
        Email = email;
    }

    public void ChangeEmail(Email newEmail)
    {
        Email = newEmail;
    }
}
