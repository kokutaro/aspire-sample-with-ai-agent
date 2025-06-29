# コーディング規約

## 1. 基本原則

### 1.1. 読みやすさを最優先

- コードは書く時間より読む時間の方が長い
- 明確で自己文書化されたコードを書く
- 複雑な処理には必ずコメントを追加

### 1.2. 一貫性を保つ

- プロジェクト全体で同じパターンを使用
- 既存のコードスタイルに従う
- 新しいパターンを導入する場合はチームで合意

## 2. 命名規則

### 2.1. C# 命名規則

- **クラス、インターフェース、メソッド、プロパティ**: PascalCase
- **ローカル変数、メソッドパラメータ**: camelCase
- **プライベートフィールド**: camelCase (アンダースコアプレフィックス `_` を推奨)
- **定数**: PascalCase (C#では通常、`const`や`static readonly`フィールドもPascalCase)
- **Enum**: PascalCase

```csharp
// クラス
public class UserService
{
    // プライベートフィールド
    private readonly IUserRepository _userRepository;

    // プロパティ
    public string UserName { get; set; }

    // メソッド
    public User GetUserById(Guid id)
    {
        // ローカル変数
        var user = _userRepository.GetById(id);
        return user;
    }
}

// インターフェース
public interface IUserRepository { /* ... */ }

// Enum
public enum UserRole
{
    Admin,
    User,
    Guest
}

// 定数
public static class Constants
{
    public const int MaxRetryCount = 3;
    public static readonly int DefaultPageSize = 20;
}
```

### 2.2. TypeScript/React 命名規則

- **変数**: camelCase
- **関数**: 動詞で始まるcamelCase
- **booleanを返す関数**: `is`/`has`/`can`で始める
- **定数**: UPPER_SNAKE_CASE
- **型・インターフェース**: PascalCase
- **ジェネリック型パラメータ**: `T`, `K`, `V`など単一文字、または説明的な名前
- **React コンポーネント**: PascalCase
- **カスタムフック**: `use`で始まるcamelCase
- **イベントハンドラ**: `handle`で始める
- **ファイル名**: コンポーネントはPascalCase (`.tsx`)、ユーティリティ・フックはcamelCase (`.ts`)、テストは`[元のファイル名].test.ts`

```typescript
// 変数名: キャメルケース
const userName = "山田太郎";
const isActive = true;
const userCount = 10;

// 関数名: 動詞で始まるキャメルケース
function getUserById(id: string) { /* ... */ }
function calculateTotalPrice(items: Item[]) { /* ... */ }

// boolean を返す関数: is/has/can で始める
function isValidEmail(email: string): boolean { /* ... */ }

// 定数: アッパースネークケース
const MAX_RETRY_COUNT = 3;
const API_BASE_URL = "/api";

// 型・インターフェース: パスカルケース
interface User {
  id: string;
  name: string;
  email: string;
}

type UserId = string;

// React コンポーネント: パスカルケース
function UserProfile({ user }: { user: User }) {
  return <div>{user.name}</div>;
}

// カスタムフック: use で始まるキャメルケース
function useUser(userId: string) { /* ... */ }

// イベントハンドラ: handle で始める
function handleClick(event: MouseEvent) { /* ... */ }

// ファイル名例
// UserProfile.tsx
// formatDate.ts
// UserProfile.test.tsx
```

## 3. TypeScript 規約

### 3.1. 型定義

- インターフェースを優先（拡張可能な場合）
- 型エイリアスはUnion型、関数型、ユーティリティ型で使用
- 明示的な`any`は避ける

### 3.2. 型ガード

- 型ガード関数を定義し、`value is Type`構文を使用

### 3.3. Nullish Coalescing と Optional Chaining

- `??` (Nullish Coalescing) と `?.` (Optional Chaining) を積極的に使用

## 4. C# 規約

### 4.1. エラーハンドリング (Resultパターン)

ドメイン層でのバリデーションエラーや操作の失敗は、例外ではなく`Result<T>`パターンを使用して表現します。これにより、呼び出し元は明示的に成功/失敗を処理でき、例外による制御フローの複雑化を避けることができます。

```csharp
// Result<T> と Error の定義は MyAspireApp.Domain.Common にて行われます。
// public record Error(string Code, string Message);
// public class Result<TValue> { /* ... */ }

// 使用例:
public class UserService
{
    public Result<User> CreateUser(string name, string email)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Result<User>.Failure(new Error("User.NameRequired", "ユーザー名は必須です。"));
        }
        // ... その他のバリデーション

        var newUser = new User(Guid.NewGuid(), name, email); // 仮のUserコンストラクタ
        return Result<User>.Success(newUser);
    }
}

// 呼び出し側
var result = userService.CreateUser("Test User", "test@example.com");
if (result.IsSuccess)
{
    Console.WriteLine($"ユーザー作成成功: {result.Value.Name}");
}
else
{
    Console.WriteLine($"エラー: {result.Error.Code} - {result.Error.Message}");
}
```

### 4.2. 依存性注入 (DI)

- コンストラクタインジェクションを推奨します。
- インターフェースを介して依存性を注入し、疎結合を保ちます。

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

## 5. 関数設計

### 5.1. 純粋関数を優先

- 副作用のない純粋関数を優先し、テスト容易性と予測可能性を高めます。

### 5.2. 単一責任の原則

- 各関数やメソッドは一つの明確な責任を持つように設計します。

### 5.3. デフォルトパラメータ

- オプションの引数にはデフォルト値を使用し、呼び出し側のコードを簡潔にします。

## 6. コメント規約

### 6.1. JSDoc (TypeScript/JavaScript)

- 関数、クラス、複雑なロジックにはJSDoc形式のコメントを使用し、引数、戻り値、例外などを記述します。

### 6.2. XML Documentation Comments (C#)

- C#の公開されたAPI（クラス、メソッド、プロパティなど）にはXMLドキュメントコメントを使用し、IntelliSenseでの情報提供を充実させます。

```csharp
/// <summary>
/// 指定されたIDのユーザーを取得します。
/// </summary>
/// <param name="userId">取得するユーザーのID。</param>
/// <returns>ユーザー情報。見つからない場合はnull。</returns>
/// <exception cref="DatabaseException">データベース接続エラーの場合にスローされます。</exception>
public async Task<User?> GetUserByIdAsync(Guid userId)
{
    // 実装
}
```

### 6.3. インラインコメント

- コードの意図や複雑なロジックを説明するために、必要に応じてインラインコメントを追加します。
- `TODO`: 未実装の機能や改善点
- `FIXME`: 既知のバグや修正が必要な箇所
- `NOTE`: 重要な注意点や設計上の考慮事項

## 7. フォーマッティング

### 7.1. Prettier (TypeScript/JavaScript/JSON)

- コードの自動フォーマットにはPrettierを使用します。設定は`.prettierrc`ファイルで管理します。

```json
{
  "semi": false,
  "singleQuote": false,
  "tabWidth": 2,
  "trailingComma": "es5",
  "printWidth": 80,
  "arrowParens": "always",
  "endOfLine": "lf"
}
```

### 7.2. EditorConfig (C#)

- C#コードのフォーマットには`.editorconfig`ファイルを使用し、IDE間で一貫したスタイルを強制します。

### 7.3. ESLint (TypeScript/JavaScript)

- コード品質の維持にはESLintを使用します。設定は`.eslintrc.js`ファイルで管理します。

```javascript
module.exports = {
  extends: [
    "next/core-web-vitals", // Next.jsを使用しない場合は適宜変更
    "plugin:@typescript-eslint/recommended",
  ],
  rules: {
    "@typescript-eslint/no-unused-vars": "error",
    "@typescript-eslint/no-explicit-any": "error",
    "@typescript-eslint/explicit-function-return-type": "off",
    "react/prop-types": "off",
    "react/react-in-jsx-scope": "off",
    "react-hooks/rules-of-hooks": "error",
    "react-hooks/exhaustive-deps": "warn",
    "no-console": ["warn", { allow: ["warn", "error"] }],
    "prefer-const": "error",
    "no-var": "error",
  },
}
```

## 8. インポート/Using 順序

### 8.1. TypeScript/JavaScript

```typescript
// 1. React/Next.js (フレームワーク固有のインポート)
import { useState, useEffect } from "react";
// import { useRouter } from "next/navigation"; // Next.jsを使用しない場合は不要

// 2. 外部ライブラリ
import { z } from "zod";
import { format } from "date-fns";

// 3. 内部ユーティリティ/lib (エイリアスパスを含む)
import { cn } from "@/lib/utils";
import { api } from "@/lib/api";

// 4. 型定義
import type { User, Post } from "@/types";

// 5. コンポーネント
import { Button } from "@/components/ui/button";
import { UserCard } from "@/components/UserCard";

// 6. スタイル
import styles from "./styles.module.css";
```

### 8.2. **C#**

- 標準ライブラリ (`System.*`)、外部ライブラリ、プロジェクト内の名前空間の順にグループ化し、各グループ内でアルファベット順にソートします。

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using MyAspireApp.Domain.Entities;
using MyAspireApp.Domain.Repositories;

// ...
```

## 9. パフォーマンス考慮事項 (React)

### 9.1. メモ化

- `useMemo`: 計算コストの高い値の再計算を防ぐ
- `useCallback`: 関数の再生成を防ぐ
- `React.memo`: コンポーネントの不要な再レンダリングを防ぐ

### 9.2. 遅延ローディング

- コンポーネントやリソースの動的インポートにより、初期ロード時間を短縮します。

```typescript
// 動的インポート
// const HeavyComponent = dynamic(() => import("./HeavyComponent"), {
//   loading: () => <Skeleton />,
//   ssr: false,
// });

// 画像の最適化
// <Image
//   src="/hero.jpg"
//   alt="Hero"
//   width={1200}
//   height={600}
//   priority={false}
//   loading="lazy"
// />
```
