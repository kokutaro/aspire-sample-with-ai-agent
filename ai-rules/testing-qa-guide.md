# テスト・品質保証ガイド

## 1. テスト戦略

### 1.1. テストピラミッド

```text
         /\
        /E2E\      - 10% (重要なユーザーフロー)
       /------\
      /統合テスト\  - 30% (API、DB連携)
     /----------\
    /  単体テスト  \ - 60% (関数、コンポーネント)
   /--------------\
```

### 1.2. テストカバレッジ目標

- 全体: 80%以上
- 重要なビジネスロジック: 95%以上
- ユーティリティ関数: 100%

## 2. 単体テスト

### 2.1. C# 単体テスト (xUnit)

C#プロジェクトの単体テストにはxUnitを使用します。ドメインロジック、アプリケーションサービス、ユーティリティ関数など、個々のコンポーネントの機能を検証します。

```csharp
// MyAspireApp.Application.Tests/UserServiceTests.cs (例)
public class UserServiceTests
{
    [Fact]
    public void CreateUser_ValidData_ReturnsSuccessResult()
    {
        // Arrange
        var userService = new UserService(); // 依存関係はモック化またはスタブ化

        // Act
        var result = userService.CreateUser("Test User", "test@example.com");

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal("Test User", result.Value.Name);
    }

    [Fact]
    public void CreateUser_InvalidEmail_ReturnsFailureResult()
    {
        // Arrange
        var userService = new UserService();

        // Act
        var result = userService.CreateUser("Test User", "invalid-email");

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("User.InvalidEmailFormat", result.Error.Code);
    }
}
```

### 2.2. TypeScript/React 単体テスト (Jest)

TypeScript/Reactプロジェクトの単体テストにはJestを使用します。コンポーネント、カスタムフック、ユーティリティ関数などの機能を検証します。

```typescript
// schemas/__tests__/user.test.ts (例)
import { UserSchema, CreateUserSchema } from '../user';

describe('UserSchema', () => {
  it('有効なユーザーデータを検証できる', () => {
    const validUser = {
      id: '550e8400-e29b-41d4-a716-446655440000',
      email: 'test@example.com',
      name: '山田太郎',
      age: 30,
      role: 'user',
      createdAt: new Date(),
    };
    expect(UserSchema.safeParse(validUser).success).toBe(true);
  });

  it('無効なメールアドレスを拒否する', () => {
    const invalidUser = {
      id: '550e8400-e29b-41d4-a716-446655440000',
      email: 'invalid-email',
      name: '山田太郎',
      role: 'user',
      createdAt: new Date(),
    };
    const result = UserSchema.safeParse(invalidUser);
    expect(result.success).toBe(false);
    expect(result.error?.issues[0].path).toContain('email');
  });
});
```

### 2.3. C# Web APIのテスト (Integration Tests)

ASP.NET Coreの`WebApplicationFactory`を使用して、インメモリでAPIをホストし、エンドツーエンドのAPIテストを行います。これにより、コントローラ、サービス、データベースアクセスを含むAPIの完全なスタックをテストできます。

```csharp
// MyAspireApp.ApiService.Tests/UsersApiTests.cs (例)
public class UsersApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public UsersApiTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Post_CreateUser_ReturnsSuccess()
    {
        // Arrange
        var client = _factory.CreateClient();
        var request = new {
            Name = "Test User",
            Email = "test@example.com"
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/users", request);

        // Assert
        response.EnsureSuccessStatusCode(); // Status Code 200-299
        var user = await response.Content.ReadFromJsonAsync<User>();
        Assert.NotNull(user);
        Assert.Equal("test@example.com", user.Email);
    }

    [Fact]
    public async Task Post_CreateUser_InvalidEmail_ReturnsBadRequest()
    {
        // Arrange
        var client = _factory.CreateClient();
        var request = new {
            Name = "Test User",
            Email = "invalid-email"
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/users", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        // ProblemDetailsの検証など
    }
}
```

### 2.4. Reactコンポーネントのテスト

```typescript
// components/__tests__/UserCard.test.tsx
import { render, screen } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { UserCard } from '../UserCard'

describe('UserCard', () => {
  const mockUser = {
    id: '1',
    name: '山田太郎',
    email: 'yamada@example.com',
    role: 'user' as const,
  }
  
  it('ユーザー情報を表示する', () => {
    render(<UserCard user={mockUser} />)
    
    expect(screen.getByText('山田太郎')).toBeInTheDocument()
    expect(screen.getByText('yamada@example.com')).toBeInTheDocument()
    expect(screen.getByText('一般ユーザー')).toBeInTheDocument()
  })
  
  it('編集ボタンクリックでコールバックが呼ばれる', async () => {
    const handleEdit = jest.fn()
    render(<UserCard user={mockUser} onEdit={handleEdit} />)
    
    const editButton = screen.getByRole('button', { name: '編集' })
    await userEvent.click(editButton)
    
    expect(handleEdit).toHaveBeenCalledWith(mockUser.id)
  })
})
```

### 2.5. カスタムフックのテスト

```typescript
// hooks/__tests__/useUser.test.ts
import { renderHook, waitFor } from '@testing-library/react'
import { useUser } from '../useUser'
import { SWRConfig } from 'swr'

const wrapper = ({ children }: { children: React.ReactNode }) => (
  <SWRConfig value={{ dedupingInterval: 0 }}>
    {children}
  </SWRConfig>
)

describe('useUser', () => {
  beforeEach(() => {
    global.fetch = jest.fn()
  })
  
  it('ユーザーデータを取得できる', async () => {
    const mockUser = {
      id: '1',
      name: '山田太郎',
      email: 'yamada@example.com',
    }
    
    ;(global.fetch as jest.Mock).mockResolvedValueOnce({
      ok: true,
      json: async () => mockUser,
    })
    
    const { result } = renderHook(() => useUser('1'), { wrapper })
    
    await waitFor(() => {
      expect(result.current.user).toEqual(mockUser)
      expect(result.current.isLoading).toBe(false)
      expect(result.current.isError).toBe(undefined)
    })
  })
})
```

## 3. 統合テスト

### 3.1. EF Coreのテスト

EF Coreの統合テストでは、実際のデータベース（テストコンテナなど）を使用して、リポジトリの実装やデータベースとのインタラクションを検証します。これにより、永続化層の正確性とパフォーマンスを確認できます。

```csharp
// MyAspireApp.Infrastructure.Tests/UserRepositoryTests.cs (例)
public class UserRepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _dbContext;
    private readonly UserRepository _userRepository;

    public UserRepositoryTests()
    {
        // インメモリデータベースまたはテストコンテナを使用
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _dbContext = new ApplicationDbContext(options);
        _userRepository = new UserRepository(_dbContext);

        _dbContext.Database.EnsureCreated();
    }

    public void Dispose()
    {
        _dbContext.Database.EnsureDeleted();
        _dbContext.Dispose();
    }

    [Fact]
    public async Task AddAndGetByIdAsync_User_ReturnsUser()
    {
        // Arrange
        var userId = UserId.New();
        var user = new User(userId, "Test User", "test@example.com");

        // Act
        _userRepository.Add(user);
        await _dbContext.SaveChangesAsync();

        var retrievedUser = await _userRepository.GetByIdAsync(userId);

        // Assert
        Assert.NotNull(retrievedUser);
        Assert.Equal(user.Id, retrievedUser.Id);
        Assert.Equal(user.Name, retrievedUser.Name);
    }

    [Fact]
    public async Task ExistsByEmail_DuplicateEmail_ReturnsTrue()
    {
        // Arrange
        var userId = UserId.New();
        var user = new User(userId, "Test User", "duplicate@example.com");
        _userRepository.Add(user);
        await _dbContext.SaveChangesAsync();

        // Act
        var exists = _userRepository.ExistsByEmail("duplicate@example.com");

        // Assert
        Assert.True(exists);
    }
}
```

## 4. E2Eテスト (Playwright)

### 4.1. Playwright設定

```typescript
// playwright.config.ts
import { defineConfig, devices } from '@playwright/test'

export default defineConfig({
  testDir: './e2e',
  fullyParallel: true,
  forbidOnly: !!process.env.CI,
  retries: process.env.CI ? 2 : 0,
  workers: process.env.CI ? 1 : undefined,
  reporter: 'html',
  use: {
    baseURL: 'http://localhost:3000',
    trace: 'on-first-retry',
    screenshot: 'only-on-failure',
  },
  projects: [
    {
      name: 'chromium',
      use: { ...devices['Desktop Chrome'] },
    },
    {
      name: 'Mobile Chrome',
      use: { ...devices['Pixel 5'] },
    },
  ],
  webServer: {
    command: 'npm run dev',
    port: 3000,
    reuseExistingServer: !process.env.CI,
  },
})
```

### 4.2. E2Eテストの実装

```typescript
// e2e/user-management.spec.ts
import { test, expect } from '@playwright/test'

test.describe('ユーザー管理', () => {
  test.beforeEach(async ({ page }) => {
    await page.goto('/users')
  })
  
  test('ユーザー一覧を表示できる', async ({ page }) => {
    await expect(page.getByRole('heading', { name: 'ユーザー一覧' })).toBeVisible()
    await expect(page.getByRole('table')).toBeVisible()
  })
  
  test('新規ユーザーを作成できる', async ({ page }) => {
    // 作成ボタンをクリック
    await page.getByRole('button', { name: '新規作成' }).click()
    
    // フォームに入力
    await page.getByLabel('名前').fill('田中花子')
    await page.getByLabel('メールアドレス').fill('tanaka@example.com')
    await page.getByLabel('役割').selectOption('user')
    
    // 送信
    await page.getByRole('button', { name: '作成' }).click()
    
    // 成功メッセージを確認
    await expect(page.getByText('ユーザーを作成しました')).toBeVisible()
    
    // 一覧に表示されることを確認
    await expect(page.getByRole('cell', { name: '田中花子' })).toBeVisible()
  })
  
  test('入力エラーを表示する', async ({ page }) => {
    await page.getByRole('button', { name: '新規作成' }).click()
    
    // 空のフォームを送信
    await page.getByRole('button', { name: '作成' }).click()
    
    // エラーメッセージを確認
    await expect(page.getByText('名前は必須です')).toBeVisible()
    await expect(page.getByText('メールアドレスは必須です')).toBeVisible()
  })
})
```

### 4.3. ビジュアルリグレッションテスト

```typescript
// e2e/visual-regression.spec.ts
import { test, expect } from '@playwright/test'

test.describe('ビジュアルリグレッション', () => {
  test('ホームページのスクリーンショット', async ({ page }) => {
    await page.goto('/')
    await expect(page).toHaveScreenshot('homepage.png', {
      fullPage: true,
      animations: 'disabled',
    })
  })
  
  test('ユーザー一覧のスクリーンショット', async ({ page }) => {
    await page.goto('/users')
    await page.waitForLoadState('networkidle')
    await expect(page).toHaveScreenshot('users-list.png')
  })
})
```

## 5. 品質チェック自動化

### 5.1. pre-commitフック

```bash
# .husky/pre-commit
#!/bin/sh
. "$(dirname "$0")/_/husky.sh"

# フォーマットチェック
npm run format:check || {
  echo "❌ フォーマットエラーが見つかりました。'npm run format'を実行してください。"
  exit 1
}

# リントチェック
npm run lint || {
  echo "❌ リントエラーが見つかりました。"
  exit 1
}

# 型チェック
npm run type-check || {
  echo "❌ 型エラーが見つかりました。"
  exit 1
}

# テスト実行（変更されたファイルのみ）
npm run test:staged || {
  echo "❌ テストが失敗しました。"
  exit 1
}
```

### 5.2. GitHub Actions CI/CD

```yaml
# .github/workflows/ci.yml
name: CI

on:
  push:
    branches: [main]
  pull_request:
    branches: [main]

jobs:
  build-and-test:
    runs-on: ubuntu-latest
    
    services:
      postgres:
        image: postgres:15
        env:
          POSTGRES_PASSWORD: mysecretpassword
          POSTGRES_DB: testdb
        options: >-
          --health-cmd pg_isready
          --health-interval 10s
          --health-timeout 5s
          --health-retries 5
        ports:
          - 5432:5432
    
    steps:
      - uses: actions/checkout@v3
      
      - name: Setup .NET
        uses: actions/setup-dotnet@v3
        with:
          dotnet-version: '9.0.x'

      - name: Setup Node.js
        uses: actions/setup-node@v3
        with:
          node-version: '20'
          cache: 'npm'
      
      - name: Install .NET dependencies
        run: dotnet restore

      - name: Install Node.js dependencies
        run: npm ci
        working-directory: src/myaspireapp-frontend

      - name: Build .NET projects
        run: dotnet build --no-restore

      - name: Run .NET tests
        run: dotnet test --no-build --verbosity normal

      - name: Run Node.js tests
        run: npm test
        working-directory: src/myaspireapp-frontend

      - name: Run E2E tests
        run: npx playwright test
        working-directory: src/myaspireapp-frontend
        env:
          API_BASE_URL: http://localhost:5000 # YARPのポートに合わせて調整

      - name: Upload test results
        if: always()
        uses: actions/upload-artifact@v3
        with:
          name: test-results
          path: |
            coverage/
            playwright-report/
```

## 6. パフォーマンステスト

### 6.1. Lighthouse CI

```javascript
// lighthouserc.js
module.exports = {
  ci: {
    collect: {
      staticDistDir: './out',
      url: [
        'http://localhost:3000/',
        'http://localhost:3000/users',
      ],
    },
    assert: {
      assertions: {
        'categories:performance': ['error', { minScore: 0.9 }],
        'categories:accessibility': ['error', { minScore: 0.95 }],
        'categories:best-practices': ['error', { minScore: 0.9 }],
        'categories:seo': ['error', { minScore: 0.9 }],
      },
    },
    upload: {
      target: 'temporary-public-storage',
    },
  },
}
```

## 7. テストデバッグ

### 7.1. デバッグ方法

```typescript
// テスト内でのデバッグ
test('デバッグが必要なテスト', async () => {
  // ブレークポイントを設定
  debugger
  
  // コンソールログ
  console.log('現在の状態:', someVariable)
  
  // スナップショット
  expect(someObject).toMatchSnapshot()
})

// Playwrightのデバッグモード
npx playwright test --debug
npx playwright test --ui
```

## 8. 品質メトリクス

### 8.1. 追跡すべき指標

- コードカバレッジ率
- 技術的負債の量
- バグ発見率
- テスト実行時間
- ビルド成功率

### 8.2. 品質ダッシュボード

```typescript
// scripts/quality-report.ts
interface QualityMetrics {
  coverage: {
    lines: number
    branches: number
    functions: number
    statements: number
  }
  testResults: {
    passed: number
    failed: number
    skipped: number
    duration: number
  }
  codeQuality: {
    duplications: number
    complexityAverage: number
    maintainabilityIndex: number
  }
}
```
