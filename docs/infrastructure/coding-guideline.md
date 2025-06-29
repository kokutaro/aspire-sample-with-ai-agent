# インフラ層コーディングガイドライン

このドキュメントは、インフラ層における特定のコーディングスタイルと実装原則をまとめたものです。

## 1. DbContextの作成ルール

アプリケーションのデータベースコンテキストは、`ApplicationDbContext`として`MyAspireApp.Infrastructure`プロジェクトのルートに配置します。この`DbContext`は、`Microsoft.EntityFrameworkCore.DbContext`を継承し、以下の原則に従います。

- **コンストラクタ**: `DbContextOptions<ApplicationDbContext>`を引数に取るコンストラクタを持ち、基底クラスに渡します。

    ```csharp
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        // ...
    }
    ```

- **`DbSet`の定義**: ドメイン層で定義された各エンティティ（例: `User`）に対応する`DbSet<TEntity>`プロパティを定義します。

    ```csharp
    public DbSet<User> Users { get; set; }
    ```

- **`OnModelCreating`メソッド**: このメソッドをオーバーライドし、モデルの構築時にエンティティの設定を適用します。特に、`ApplyConfigurationsFromAssembly`を使用して、アセンブリ内のすべての`IEntityTypeConfiguration`実装を自動的に適用します。

    ```csharp
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // アセンブリ内のすべてのエンティティ設定を適用
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        // StronglyTypedIdの自動変換を適用
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType.IsGenericType &&
                    property.ClrType.GetGenericTypeDefinition() == typeof(StronglyTypedId<>) &&
                    property.ClrType.GetGenericArguments()[0] == typeof(Guid))
                {
                    var converterType = typeof(StronglyTypedIdConverter<>).MakeGenericType(property.ClrType);
                    var converter = (ValueConverter)Activator.CreateInstance(converterType)!;
                    property.SetValueConverter(converter);
                }
            }
        }
    }
    ```

## 2. エンティティの設定ルール (`IEntityTypeConfiguration<TEntity>`)

各エンティティのEF Coreにおけるマッピング設定は、`IEntityTypeConfiguration<TEntity>`インターフェースを実装した個別のクラスで行います。これらの設定クラスは、`MyAspireApp.Infrastructure/EntityConfigurations`フォルダ以下に配置します。

- **命名規則**: `[EntityName]Configuration.cs`（例: `UserConfiguration.cs`）
- **実装**: `IEntityTypeConfiguration<TEntity>`インターフェースの`Configure`メソッドを実装し、`EntityTypeBuilder<TEntity>`を使用してエンティティのプロパティ、リレーションシップ、インデックスなどを設定します。
- **Strongly Typed IDの変換**: `StronglyTypedId<Guid>`型のIDプロパティについては、汎用的な`StronglyTypedIdConverter`が`ApplicationDbContext`の`OnModelCreating`で自動的に適用されるため、個別の`HasConversion`設定は不要です。

**例: `UserConfiguration.cs`**

```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyAspireApp.Domain.Entities;

namespace MyAspireApp.Infrastructure.EntityConfigurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(u => u.Id);

        // StronglyTypedIdConverterが自動適用されるため、個別のHasConversionは不要
        // builder.Property(u => u.Id)
        //     .HasConversion(v => v.Value, v => new UserId(v));

        builder.Property(u => u.Name)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(255);

        builder.HasIndex(u => u.Email).IsUnique();
    }
}
```

## 3. 汎用的なStronglyTypedIdコンバーター

`StronglyTypedId<TId>`と`TId`間の変換を自動化するため、`StronglyTypedIdConverter<TStronglyTypedId>`を定義し、`ApplicationDbContext`の`OnModelCreating`でリフレクションを用いて適用します。

```csharp
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MyAspireApp.Domain.Common;

namespace MyAspireApp.Infrastructure.Converters;

public class StronglyTypedIdConverter<TStronglyTypedId> : ValueConverter<TStronglyTypedId, Guid>
    where TStronglyTypedId : StronglyTypedId<Guid>
{
    public StronglyTypedIdConverter(ConverterMappingHints? mappingHints = null)
        : base(
            id => id.Value,
            value => (TStronglyTypedId)Activator.CreateInstance(typeof(TStronglyTypedId), value)!,
            mappingHints)
    {
    }
}
```

このアプローチにより、新しいエンティティを追加する際に、そのIDに対するEF Coreの変換設定を手動で記述する必要がなくなります。
