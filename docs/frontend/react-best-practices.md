# Reactアプリケーションのベストプラクティス (Vite環境向け)

このドキュメントでは、Viteを使ったReactアプリケーションの持続可能な開発環境を構築するためのベストプラクティスをまとめます。

## 1. フォルダ構成

大規模なアプリケーションでは、機能やドメインごとにファイルをまとめる「Feature-Sliced Design」や「Domain-Driven Design」のようなアプローチが有効です。小規模なアプリケーションでは、タイプごとにファイルをまとめる「Type-Based」な構成もシンプルで分かりやすいです。

### 推奨される構成 (Feature-Sliced Designの概念を取り入れつつシンプルに)

```text
src/
├── assets/             # 静的ファイル、画像など
├── components/         # 再利用可能なUIコンポーネント（汎用的なもの）
│   ├── Button/
│   │   ├── Button.tsx
│   │   ├── Button.module.css
│   │   └── index.ts    # エクスポート用
│   └── ...
├── features/           # 特定の機能（ドメイン）ごとのコンポーネント、ロジック
│   ├── Auth/
│   │   ├── components/ # Auth機能内でしか使わないコンポーネント
│   │   │   └── LoginForm.tsx
│   │   ├── hooks/
│   │   │   └── useAuth.ts
│   │   ├── api/
│   │   │   └── authApi.ts
│   │   └── index.ts    # Auth機能のエントリポイント
│   └── UserProfile/
│       └── ...
├── hooks/              # カスタムフック（汎用的なもの）
├── layouts/            # ページのレイアウトコンポーネント
├── pages/              # ルーティングされるページコンポーネント
│   ├── HomePage.tsx
│   └── AboutPage.tsx
├── services/           # APIクライアント、外部サービス連携など
├── store/              # 状態管理（Redux, Zustand, Context APIなど）
├── utils/              # ユーティリティ関数
├── App.tsx             # アプリケーションのメインコンポーネント
├── main.tsx            # エントリポイント
└── vite-env.d.ts       # Vite環境の型定義
```

**ポイント:**

* **`components`**: アプリケーション全体で再利用される汎用的なUIコンポーネントを配置します。
* **`features`**: 特定のビジネスロジックやドメインに密接に関連するコンポーネント、フック、API呼び出しなどをまとめて配置します。これにより、機能ごとの独立性が高まり、変更の影響範囲を限定しやすくなります。
* **`pages`**: ルーティングによって表示されるトップレベルのコンポーネントを配置します。
* **`hooks`, `services`, `utils`**: アプリケーション全体で利用される汎用的なロジックやヘルパー関数を配置します。

## 2. コンポーネントの作り方

### 2.1. コンポーネントの種類

* **プレゼンテーションコンポーネント (Presentational Components)**:
  * UIの見た目を担当します。
  * 状態を持たず、propsを通じてデータを受け取ります。
  * ビジネスロジックを含まず、イベントハンドラもpropsで受け取ります。
  * 例: `Button`, `Card`, `Modal`
* **コンテナコンポーネント (Container Components)**:
  * ビジネスロジックや状態管理を担当します。
  * データをフェッチしたり、状態を管理し、プレゼンテーションコンポーネントにpropsとして渡します。
  * 例: `UserListContainer`, `ProductDetailContainer`

### 2.2. 関数コンポーネントとHooks

* React 16.8以降、関数コンポーネントとHooksの使用が推奨されています。クラスコンポーネントよりも簡潔で、ロジックの再利用が容易です。
* 状態管理には`useState`、副作用には`useEffect`、パフォーマンス最適化には`useMemo`, `useCallback`などを適切に使用します。

### 2.3. スタイリング

* **CSS Modules**: コンポーネントごとにスコープされたCSSを記述でき、クラス名の衝突を防ぎます。Viteでデフォルトでサポートされています。
  * 例: `Button.module.css`
* **CSS-in-JS (例: Styled Components, Emotion)**: JavaScript内でCSSを記述する方法です。コンポーネントとスタイルを密接に結合できます。
* **Utility-First CSS (例: Tailwind CSS)**: 事前定義されたユーティリティクラスをHTMLに直接適用する方法です。高速な開発が可能ですが、HTMLが冗長になる場合があります。

プロジェクトの規模やチームの好みに合わせて選択します。

### 2.4. Propsの型定義 (TypeScript)

TypeScriptを使用している場合、Propsの型定義は必須です。これにより、コンポーネントのインターフェースが明確になり、開発時のエラーを防ぎます。

```typescript
interface ButtonProps {
  onClick: () => void;
  children: React.ReactNode;
  variant?: 'primary' | 'secondary';
  disabled?: boolean;
}

const Button: React.FC<ButtonProps> = ({ onClick, children, variant = 'primary', disabled = false }) => {
  return (
    <button className={`button button--${variant}`} onClick={onClick} disabled={disabled}>
      {children}
    </button>
  );
};
```

## 3. 各ファイルの命名規則

一貫性のある命名規則は、コードの可読性と保守性を高めます。

* **コンポーネント**: PascalCase (例: `MyComponent.tsx`, `LoginForm.tsx`)
* **カスタムフック**: `use`で始める (例: `useAuth.ts`, `useDebounce.ts`)
* **スタイルファイル**:
  * CSS Modules: `[ComponentName].module.css` (例: `Button.module.css`)
  * 通常のCSS: `[ComponentName].css` (例: `Button.css`)
* **ユーティリティ関数**: camelCase (例: `formatDate.ts`)
* **インデックスファイル**: `index.ts` (フォルダのエントリポイントとして使用し、内部のコンポーネントや関数をエクスポートする)

## 4. その他のベストプラクティス

* **状態管理**:
  * 小規模な状態には`useState`やContext APIを、グローバルな状態や複雑な状態にはRedux, Zustand, Recoilなどのライブラリを検討します。
* **ルーティング**:
  * `react-router-dom`がデファクトスタンダードです。
* **API通信**:
  * `fetch` APIのラッパーや、`axios`, `react-query` (データフェッチライブラリ) などを利用します。`react-query`はキャッシュ、再フェッチ、エラーハンドリングなどを強力にサポートします。
* **テスト**:
  * コンポーネントのテストには`React Testing Library`と`Jest`の組み合わせが推奨されます。
  * E2Eテストには`Cypress`や`Playwright`を検討します。
* **Linting & Formatting**:
  * `ESLint`と`Prettier`を導入し、コードの品質と一貫性を保ちます。Viteプロジェクトではこれらが初期設定に含まれていることが多いです。
* **TypeScript**:
  * 可能な限りTypeScriptを使用し、型安全なコードを記述します。
