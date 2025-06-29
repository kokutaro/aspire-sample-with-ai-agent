# 技術スタックガイドライン

このドキュメントは、プロジェクトで使用される主要な技術スタックに関するベストプラクティスとガイドラインをまとめたものです。

## 1. .NET Web API ベストプラクティス

### 1.1. プロジェクト構造

バックエンドAPIは、ドメイン駆動設計（DDD）とクリーンアーキテクチャの原則に基づいて層別化されています。これにより、ビジネスロジックと技術的な詳細が明確に分離され、保守性と拡張性が向上します。

- **`MyAspireApp.Domain`**: ドメイン層。ビジネスのコアなルール、エンティティ、値オブジェクト、ドメインイベント、リポジトリのインターフェースを定義します。他の層への依存は持ちません。
- **`MyAspireApp.Application`**: アプリケーション層。ドメイン層を操作するユースケース（アプリケーションサービス）を定義します。DTOや外部インフラ（メール送信、決済など）のインターフェースを含みます。`MyAspireApp.Domain`に依存します。
- **`MyAspireApp.Infrastructure`**: インフラストラクチャ層。`MyAspireApp.Application`層や`MyAspireApp.Domain`層で定義されたインターフェースの具体的な実装を提供します。データベースアクセス（Entity Framework Core）、外部APIクライアント、キャッシュの実装などが含まれます。`MyAspireApp.Application`に依存します。
- **`MyAspireApp.ApiService`**: プレゼンテーション層（Web API）。外部からのリクエストを受け取り、アプリケーション層のユースケースを呼び出します。`MyAspireApp.Application`と`MyAspireApp.Infrastructure`に依存します。

### 1.2. 依存性注入 (DI)

- **コンストラクタインジェクション**: 依存性の注入にはコンストラクタインジェクションを推奨します。
- **インターフェースと実装の分離**: サービスはインターフェースを介して依存性を注入し、疎結合を保ちます。

```csharp
public class MyService
{
    private readonly IMyDependency _dependency;

    public MyService(IMyDependency dependency)
    {
        _dependency = dependency;
    }

    public void DoSomething() { /* ... */ }
}
```

### 1.3. エラーハンドリング

- **Resultパターン**: ドメイン層およびアプリケーション層でのバリデーションエラーや操作の失敗は、例外ではなく`Result<T>`パターンを使用して表現します。これにより、呼び出し元は明示的に成功/失敗を処理でき、例外による制御フローの複雑化を避けることができます。
- **ProblemDetails**: APIレスポンスとしてエラーを返す際には、RFC 7807で定義されているProblem Details (`Microsoft.AspNetCore.Mvc.ProblemDetails`) を使用し、一貫性のあるエラー形式を提供します。

### 1.4. データベースアクセス (Entity Framework Core)

- **DbContextの管理**: `ApplicationDbContext`を`MyAspireApp.Infrastructure`プロジェクトのルートに配置し、データベースとのインタラクションを管理します。
- **EntityConfigurations**: 各エンティティのEF Coreマッピング設定は、`IEntityTypeConfiguration<TEntity>`インターフェースを実装した個別のクラスで行い、`MyAspireApp.Infrastructure/EntityConfigurations`フォルダに配置します。
- **汎用StronglyTypedIdコンバーター**: `StronglyTypedId<Guid>`型のIDプロパティについては、汎用的な`StronglyTypedIdConverter`が`ApplicationDbContext`の`OnModelCreating`でリフレクションを用いて自動的に適用されます。

## 2. YARP (Yet Another Reverse Proxy) ベストプラクティス

### 2.1. 設定

- **`appsettings.json`**: YARPのルーティングとクラスター定義は、`MyAspireApp.Web`プロジェクトの`appsettings.json`の`ReverseProxy`セクションで行います。
- **Aspireサービスディスカバリとの連携**: `ClusterId`で指定されたサービス名（例: `apiservice`, `frontend`）は、Aspireのサービスディスカバリによって自動的に解決され、適切なエンドポイントにルーティングされます。

### 2.2. ルーティング戦略

- **APIルーティング**: `/api/{**catch-all}`のようなパスパターンを使用して、バックエンドAPIサービスにリクエストをルーティングします。
- **フロントエンドルーティング**: それ以外のリクエストは、フロントエンドアプリケーションにルーティングします。

## 3. React + Vite ベストプラクティス

### 3.1. プロジェクト構造

- **`src/` ディレクトリ**: コンポーネント、カスタムフック、ユーティリティ、型定義などを`src/`ディレクトリ内に整理します。
- **Viteの機能活用**: Viteの高速な開発サーバー、Hot Module Replacement (HMR)、Fast Refreshなどの機能を最大限に活用し、開発効率を高めます。

### 3.2. コンポーネント設計

- **関数コンポーネントとHooks**: すべての新しいコンポーネントは関数コンポーネントとしてHooks (`useState`, `useEffect`, `useContext`など) を使用して記述します。
- **Propsの型定義**: TypeScriptを使用して、コンポーネントのPropsを明確に型定義します。
- **コンポーネントの責務分離**: 各コンポーネントは単一の責務を持つように設計し、再利用性とテスト容易性を高めます。

### 3.3. 状態管理

- プロジェクトの規模や要件に応じて、Redux Toolkit, Zustand, React Contextなどの適切な状態管理ライブラリを選択し、一貫したパターンで利用します。

### 3.4. API通信

- **`fetch` APIまたはAxios**: バックエンドAPIとの通信には、標準の`fetch` APIまたはAxiosのようなHTTPクライアントライブラリを使用します。
- **エラーハンドリング**: APIからのエラーレスポンスは適切に処理し、ユーザーにフィードバックを提供します。

## 4. PostgreSQL ベストプラクティス

### 4.1. スキーマ設計

- **適切なデータ型選択**: 各カラムには、格納するデータに最適なPostgreSQLのデータ型を選択します。
- **インデックスの利用**: クエリのパフォーマンスを向上させるために、頻繁に検索されるカラムや結合に使用されるカラムにはインデックスを適切に利用します。
- **外部キー制約**: 関連するテーブル間には外部キー制約を設定し、データの整合性を保ちます。

### 4.2. 接続管理

- **Aspireによる接続文字列の管理**: PostgreSQLへの接続文字列は、Aspireによって自動的に管理・注入されます。アプリケーションコード内で直接接続文字列をハードコードしないようにします。

## 5. Redis ベストプラクティス

### 5.1. キャッシュ戦略

- **キャッシュ対象の選定**: 頻繁にアクセスされるが更新頻度の低いデータ（例: 設定情報、マスターデータ）をキャッシュの対象とします。
- **キャッシュの有効期限 (TTL)**: キャッシュされたデータには適切な有効期限（Time-To-Live, TTL）を設定し、データの鮮度とパフォーマンスのバランスを取ります。
- **キャッシュ無効化戦略**: データが更新された際には、関連するキャッシュエントリを適切に無効化する戦略を検討します。

### 5.2. 接続管理

- **Aspireによる接続文字列の管理**: Redisへの接続文字列も、Aspireによって自動的に管理・注入されます。アプリケーションコード内で直接接続文字列をハードコードしないようにします。
