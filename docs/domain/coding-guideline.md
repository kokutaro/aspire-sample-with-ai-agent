# コーディングガイドライン

このドキュメントは、プロジェクトにおけるコーディングスタイル、アーキテクチャの原則、および特定の技術スタックに関するガイドラインをまとめたものです。

## プロジェクト構造とクリーンアーキテクチャ

プロジェクトはドメイン駆動設計（DDD）とクリーンアーキテクチャの原則に基づいて構築されています。これにより、ビジネスロジックと技術的な詳細が明確に分離され、保守性と拡張性が向上します。

主要な層とプロジェクトは以下の通りです。

- **`MyAspireApp.Domain`**: ドメイン層。ビジネスのコアなルール、エンティティ、値オブジェクト、ドメインイベント、リポジトリのインターフェースを定義します。他の層への依存は持ちません。
- **`MyAspireApp.Application`**: アプリケーション層。ドメイン層を操作するユースケース（アプリケーションサービス）を定義します。DTOや外部インフラ（メール送信、決済など）のインターフェースを含みます。`MyAspireApp.Domain`に依存します。
- **`MyAspireApp.Infrastructure`**: インフラストラクチャ層。`MyAspireApp.Application`層や`MyAspireApp.Domain`層で定義されたインターフェースの具体的な実装を提供します。データベースアクセス（Entity Framework Core）、外部APIクライアント、キャッシュの実装などが含まれます。`MyAspireApp.Application`に依存します。
- **`MyAspireApp.ApiService`**: プレゼンテーション層（Web API）。外部からのリクエストを受け取り、アプリケーション層のユースケースを呼び出します。`MyAspireApp.Application`と`MyAspireApp.Infrastructure`に依存します。
- **`MyAspireApp.Web`**: リバースプロキシ層（YARP）。フロントエンドとバックエンドAPIへのルーティングを管理します。
- **`MyAspireApp.AppHost`**: Aspireホストプロジェクト。すべてのサービス（API, YARP, Frontend, PostgreSQL, Redisなど）のオーケストレーションとデプロイメントを定義します。

## ドメイン層の設計原則

ドメイン層はビジネスロジックの核心であり、以下の原則に基づいて設計されます。

### ドメインプリミティブの基底クラス

ドメイン層の型安全性を高めるため、エンティティのIDには`StronglyTypedId<TValue>`パターンを導入しています。これにより、異なるエンティティのIDが誤って混同されることを防ぎます。

#### `StronglyTypedId<TValue>`

`StronglyTypedId<TValue>`は、特定のエンティティのIDであることを型システムで強制するための抽象レコードクラスです。`Guid`などのプリミティブ型をラップし、そのIDが何のエンティティに属するかを明確にします。

```csharp
namespace MyAspireApp.Domain.Common;

public abstract record class StronglyTypedId<TValue> : IComparable<StronglyTypedId<TValue>>, IEquatable<StronglyTypedId<TValue>>
    where TValue : notnull
{
    public TValue Value { get; protected init; }

    protected StronglyTypedId(TValue value)
    {
        Value = value;
    }

    public override string ToString() => Value.ToString()!;

    public int CompareTo(StronglyTypedId<TValue> other) => Comparer<TValue>.Default.Compare(Value, other.Value);
}
```

#### `Entity<TId>`

`Entity<TId>`は、ドメインエンティティの基底クラスです。`StronglyTypedId<TId>`をIDとして持ち、同一性（Identity）によって区別されます。

```csharp
using MyAspireApp.Domain.Common;

namespace MyAspireApp.Domain.Entities;

public abstract class Entity<TId>
    where TId : StronglyTypedId<Guid>
{
    public TId Id { get; protected set; }

    protected Entity(TId id)
    {
        Id = id;
    }

    // For ORM or serialization
    protected Entity() { }

    public override bool Equals(object? obj)
    {
        if (obj is not Entity<TId> other)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        if (GetType() != other.GetType())
        {
            return false;
        }

        return Id.Equals(other.Id);
    }

    public override int GetHashCode() => Id.GetHashCode();

    public static bool operator ==(Entity<TId> left, Entity<TId> right)
    {
        if (ReferenceEquals(left, null) && ReferenceEquals(right, null))
        {
            return true;
        }

        if (ReferenceEquals(left, null) || ReferenceEquals(right, null))
        {
            return false;
        }

        return left.Equals(right);
    }

    public static bool operator !=(Entity<TId> left, Entity<TId> right) => !(left == right);
}
```

#### `ValueObject`

`ValueObject`は、属性の集合によって識別されるオブジェクトの基底クラスです。同一性ではなく、属性の等価性によって区別されます。例えば、住所や金額など、それ自体がユニークなIDを持たず、その構成要素の値が同じであれば同じものとみなされるような概念に適用します。

```csharp
using System.Linq;

namespace MyAspireApp.Domain.ValueObjects;

public abstract class ValueObject : IEquatable<ValueObject>
{
    protected abstract IEnumerable<object> GetEqualityComponents();

    public override bool Equals(object? obj)
    {
        if (obj is null || obj.GetType() != GetType())
        {
            return false;
        }

        var other = (ValueObject)obj;

        return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
    }

    public override int GetHashCode()
    {
        return GetEqualityComponents()
            .Select(x => x?.GetHashCode() ?? 0)
            .Aggregate((x, y) => x ^ y);
    }

    public bool Equals(ValueObject? other)
    {
        return Equals((object?)other);
    }

    public static bool operator ==(ValueObject left, ValueObject right)
    {
        if (ReferenceEquals(left, null) && ReferenceEquals(right, null))
        {
            return true;
        }

        if (ReferenceEquals(left, null) || ReferenceEquals(right, null))
        {
            return false;
        }

        return left.Equals(right);
    }

    public static bool operator !=(ValueObject left, ValueObject right) => !(left == right);
}
```

### ドメインプリミティブの使用例

#### StronglyTypedIdとEntityの使用例

`StronglyTypedId`と`Entity`の具体的な使用例は以下のようになります。これらは同じファイル内に定義してください。

```csharp
namespace MyAspireApp.Domain.Entities;

// ユーザーIDのStrongly Typed IDを定義
public sealed record UserId(Guid Id) : StronglyTypedId<Guid>(Id);

// ユーザーエンティティを定義
public class User : Entity<UserId>
{
    public string Name { get; private set; }

    // ORM用コンストラクタ
    private User() { }

    internal User(UserId id, string name) : base(id)
    {
        Name = name;
    }

    public void ChangeName(string newName)
    {
        Name = newName;
    }
}
```

#### ValueObjectの使用例

`ValueObject`の具体的な使用例は以下のようになります。

```csharp
// 住所を表す値オブジェクト
public class Address : ValueObject
{
    public string Street { get; }
    public string City { get; }
    public string State { get; }
    public string ZipCode { get; }

    public Address(string street, string city, string state, string zipCode)
    {
        Street = street;
        City = city;
        State = state;
        ZipCode = zipCode;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Street;
        yield return City;
        yield return State;
        yield return ZipCode;
    }
}

// 金額を表す値オブジェクト
public class Money : ValueObject
{
    public decimal Amount { get; }
    public string Currency { get; }

    public Money(decimal amount, string currency)
    {
        if (amount < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(amount), "Amount cannot be negative.");
        }
        if (string.IsNullOrWhiteSpace(currency))
        {
            throw new ArgumentException("Currency cannot be null or empty.", nameof(currency));
        }

        Amount = amount;
        Currency = currency;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Amount;
        yield return Currency;
    }

    public Money Add(Money other)
    {
        if (Currency != other.Currency)
        {
            throw new InvalidOperationException("Cannot add money of different currencies.");
        }
        return new Money(Amount + other.Amount, Currency);
    }
}

// エンティティでの使用例
public class Order : Entity<OrderId>
{
    public Address ShippingAddress { get; private set; }
    public Money TotalAmount { get; private set; }

    // ORM用コンストラクタ
    private Order() { }

    public Order(OrderId id, Address shippingAddress, Money totalAmount) : base(id)
    {
        ShippingAddress = shippingAddress;
        TotalAmount = totalAmount;
    }

    public void UpdateShippingAddress(Address newAddress)
    {
        ShippingAddress = newAddress;
    }

    public void AddAmount(Money amountToAdd)
    {
        TotalAmount = TotalAmount.Add(amountToAdd);
    }
}

// 使用例
var address1 = new Address("123 Main St", "Anytown", "CA", "90210");
var address2 = new Address("123 Main St", "Anytown", "CA", "90210");
var address3 = new Address("456 Oak Ave", "Othertown", "NY", "10001");

Console.WriteLine(address1 == address2); // True (値が同じなので等しい)
Console.WriteLine(address1 == address3); // False

var price1 = new Money(100, "USD");
var price2 = new Money(50, "USD");
var totalPrice = price1.Add(price2); // totalPriceは150 USD
```

### エンティティの作成と不変性 (Builderパターン)

エンティティの作成は、その整合性を保証するためにBuilderパターンを使用することを推奨します。エンティティのコンストラクタは`internal`以下に設定し、他層から直接インスタンス化することを禁止します。これにより、エンティティの生成ルールを強制し、不整合な状態でのエンティティ作成を防ぎます。

```csharp
// 例: UserエンティティのBuilder
public class UserBuilder
{
    private UserId _id;
    private string _name;
    private string _email;

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
        // ここでバリデーションを行う
        if (_id == null) return Result<User>.Failure(new Error("UserBuilder.IdRequired", "User ID is required."));
        if (string.IsNullOrWhiteSpace(_name)) return Result<User>.Failure(new Error("UserBuilder.NameRequired", "User name is required."));
        if (string.IsNullOrWhiteSpace(_email)) return Result<User>.Failure(new Error("UserBuilder.EmailRequired", "User email is required."));

        return Result<User>.Success(new User(_id, _name, _email));
    }
}
```

### バリデーションとエラーハンドリング (Resultパターン)

ドメイン層でのバリデーションエラーや操作の失敗は、例外ではなく`Result<T>`パターンを使用して表現します。これにより、呼び出し元は明示的に成功/失敗を処理でき、例外による制御フローの複雑化を避けることができます。

```csharp
namespace MyAspireApp.Domain.Common;

public record Error(string Code, string Message);

public class Result<TValue>
{
    private readonly TValue? _value;
    private readonly Error? _error;

    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;

    public TValue Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("Cannot access value when result is a failure.");

    public Error Error => IsFailure
        ? _error!
        : throw new InvalidOperationException("Cannot access error when result is a success.");

    private Result(TValue value)
    {
        IsSuccess = true;
        _value = value;
        _error = default;
    }

    private Result(Error error)
    {
        IsSuccess = false;
        _value = default;
        _error = error;
    }

    public static Result<TValue> Success(TValue value) => new(value);
    public static Result<TValue> Failure(Error error) => new(error);

    public static implicit operator Result<TValue>(TValue value) => Success(value);
    public static implicit operator Result<TValue>(Error error) => Failure(error);
}
```

**使用例:**

```csharp
public class User : Entity<UserId>
{
    // ...

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

    private bool IsValidEmail(string email) => email.Contains("@"); // 簡易的な例
}

// アプリケーション層での使用
var user = userRepository.GetById(userId);
var result = user.ChangeEmail("new.email@example.com");

if (result.IsSuccess)
{
    // 成功時の処理
}
else
{
    Console.WriteLine($"Error: {result.Error.Code} - {result.Error.Message}");
}
```

### ドメインサービス (Domain Service)

複数のエンティティを跨いだ連携や、特定のエンティティに属さないドメインロジックは、ドメインサービスとして定義します。ドメインサービスはステートレスであり、ドメインの概念を表現する操作を提供します。

```csharp
namespace MyAspireApp.Domain.Services;

public interface IDomainService
{
    // マーカーインターフェース
}

// 例: ユーザー登録サービス
public class UserRegistrationService : IDomainService
{
    private readonly IUserRepository _userRepository;
    private readonly IEmailService _emailService; // 外部サービスインターフェース

    public UserRegistrationService(IUserRepository userRepository, IEmailService emailService)
    {
        _userRepository = userRepository;
        _emailService = emailService;
    }

    public Result<User> RegisterUser(string name, string email, string password)
    {
        // ドメインロジック: ユーザー名やメールアドレスの重複チェックなど
        if (_userRepository.ExistsByEmail(email))
        {
            return Result<User>.Failure(new Error("User.EmailAlreadyExists", "Email already registered."));
        }

        var userId = UserId.New();
        var user = new UserBuilder()
            .WithId(userId)
            .WithName(name)
            .WithEmail(email)
            .Build(); // Builderパターンでエンティティを作成

        _userRepository.Add(user);
        _emailService.SendWelcomeEmail(user.Email, user.Name); // 外部サービス呼び出し

        return Result<User>.Success(user);
    }
}
```

### リポジトリパターン (Repository Pattern)

データアクセスはリポジトリパターンを使用します。リポジトリの抽象（インターフェース）はドメイン層で定義し、インフラ層がその具体的な実装を提供します。これにより、ドメイン層は永続化の詳細から完全に分離されます。

```csharp
using System.Linq.Expressions;
using MyAspireApp.Domain.Entities;
using MyAspireApp.Domain.Common;

namespace MyAspireApp.Domain.Repositories;

public interface IRepository<TEntity, TId>
    where TEntity : Entity<TId>
    where TId : StronglyTypedId<Guid>
{
    Task<TEntity?> GetByIdAsync(TId id);
    Task<IEnumerable<TEntity>> GetAllAsync();
    Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate);
    void Add(TEntity entity);
    void Update(TEntity entity);
    void Remove(TEntity entity);
}

// 例: ユーザーリポジトリのインターフェース
public interface IUserRepository : IRepository<User, UserId>
{
    bool ExistsByEmail(string email);
    // 特定のクエリメソッドを追加
}
```

**インフラ層での実装例 (概念):**

```csharp
// MyAspireApp.Infrastructure プロジェクト内
public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _dbContext; // EF CoreのDbContext

    public UserRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<User?> GetByIdAsync(UserId id)
    {
        return await _dbContext.Users.FindAsync(id);
    }

    public bool ExistsByEmail(string email)
    {
        return _dbContext.Users.Any(u => u.Email == email);
    }

    // ... その他のIRepositoryメソッドの実装
}
```

## Aspireとサービスディスカバリ

Aspireを利用してマイクロサービスをオーケストレーションしています。サービス間の通信には、Aspireのサービスディスカバリ機能を利用します。

- **`MyAspireApp.AppHost`**: 各サービスの定義と依存関係を管理します。`AddProject`, `AddPostgresContainer`, `AddRedisContainer`, `AddNpmApp`などを使用してサービスを登録します。
- **サービス間の参照**: `WithReference`メソッドを使用して、サービス間の依存関係を定義します。これにより、環境変数や接続文字列が自動的に注入されます。
- **YARPによるルーティング**: `MyAspireApp.Web`プロジェクトはYARPをリバースプロキシとして使用し、`appsettings.json`の`ReverseProxy`セクションでルーティングルールを定義します。Aspireのサービスディスカバリにより、`ClusterId`で指定されたサービス名が自動的に解決されます。

## その他のコーディングスタイル

- **命名規則**: C#の標準的な命名規則（PascalCase, camelCaseなど）に従います。
- **インデント**: 4スペースを使用します。
- **コメント**: コードの意図や複雑なロジックを説明するために、必要に応じてコメントを追加します。
- **エラーハンドリング**: 例外処理は適切に行い、ユーザーに分かりやすいエラーメッセージを提供します。
- **非同期プログラミング**: 非同期処理には`async`/`await`パターンを積極的に使用します。

このガイドラインは、プロジェクトの進行とともに更新される可能性があります。
