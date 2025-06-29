
using System.Text.RegularExpressions;
using MyAspireApp.Domain.Exceptions;

namespace MyAspireApp.Domain.ValueObjects;

public partial class Email : ValueObject
{
    private const string EmailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";

    public string Value { get; }

    private Email(string value)
    {
        Value = value;
    }

    public static Email Create(string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new DomainException("Email cannot be empty.");
        }

        if (!EmailRegex().IsMatch(email))
        {
            throw new DomainException("Invalid email format.");
        }

        return new Email(email);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    [GeneratedRegex(EmailPattern)]
    private static partial Regex EmailRegex();
}
