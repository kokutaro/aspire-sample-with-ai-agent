# OpenAPIを利用したバックエンドとフロントエンドの型共有

このドキュメントでは、.NETバックエンドのOpenAPI定義を利用して、ReactフロントエンドでTypeScriptの型定義とAPIクライアントを自動生成する方法を説明します。これにより、バックエンドとフロントエンド間の型安全性を確保し、開発効率を向上させます。

## 1. 前提条件

* バックエンド (.NET Aspire AppService) がOpenAPI定義を公開していること。
  * `MyAspireApp.ApiService/Program.cs`に`builder.Services.AddOpenApi();`と`app.MapOpenApi();`が設定されていることを確認してください。
* フロントエンド (React) プロジェクトがTypeScriptを使用していること。

## 2. OpenAPI定義の取得

開発環境でバックエンドアプリケーションを実行すると、通常以下のURLでOpenAPI定義のJSONファイルが公開されます。

`http://localhost:<ApiServicePort>/swagger/v1/swagger.json`

`<ApiServicePort>`は、`MyAspireApp.ApiService`が起動しているポート番号です。これは、Aspireダッシュボードや`launchSettings.json`で確認できます。

## 3. `openapi-generator-cli`の導入

`openapi-generator-cli`は、OpenAPI定義から様々な言語のコードを生成できるツールです。今回はTypeScriptのAPIクライアントを生成します。

フロントエンドプロジェクト (`myaspireapp-frontend`) のルートディレクトリで、以下のコマンドを実行して`openapi-generator-cli`を開発依存関係としてインストールします。

```bash
cd MyAspireApp/src/myaspireapp-frontend
npm install @openapitools/openapi-generator-cli --save-dev
```

## 4. コード生成スクリプトの作成

`package.json`に、OpenAPI定義からコードを生成するためのスクリプトを追加します。

`MyAspireApp/src/myaspireapp-frontend/package.json`を開き、`scripts`セクションに以下のスクリプトを追加します。

```json
{
  "name": "myaspireapp-frontend",
  "private": true,
  "version": "0.0.0",
  "type": "module",
  "scripts": {
    "dev": "vite",
    "build": "tsc && vite build",
    "lint": "eslint . --ext ts,tsx --report-unused-disable-directives --max-warnings 0",
    "preview": "vite preview",
    "generate-api": "openapi-generator-cli generate -i http://localhost:<ApiServicePort>/swagger/v1/swagger.json -g typescript-fetch -o ./src/api --skip-validate-spec"
  },
  "dependencies": {
    "react": "^18.2.0",
    "react-dom": "^18.2.0"
  },
  "devDependencies": {
    "@types/react": "^18.2.66",
    "@types/react-dom": "^18.2.22",
    "@typescript-eslint/eslint-plugin": "^7.2.0",
    "@typescript-eslint/parser": "^7.2.0",
    "@vitejs/plugin-react": "^4.2.1",
    "eslint": "^8.57.0",
    "eslint-plugin-react-hooks": "^4.6.0",
    "eslint-plugin-react-refresh": "^0.4.6",
    "typescript": "^5.2.2",
    "vite": "^5.2.0",
    "@openapitools/openapi-generator-cli": "^2.13.1"
  }
}
```

**スクリプトの説明:**

* `generate-api`: このスクリプトを実行すると、APIクライアントコードが生成されます。
  * `-i http://localhost:<ApiServicePort>/swagger/v1/swagger.json`: OpenAPI定義のURLを指定します。**`<ApiServicePort>`は、実際のApiServiceのポート番号に置き換えてください。**
  * `-g typescript-fetch`: `typescript-fetch`ジェネレータを使用します。これは、`fetch` APIベースのTypeScriptクライアントコードを生成します。
  * `-o ./src/api`: 生成されたコードの出力先ディレクトリを指定します。`./src/api`に生成されます。
  * `--skip-validate-spec`: OpenAPI定義のバリデーションをスキップします。開発中に一時的に使用することがありますが、本番環境ではバリデーションを有効にすることをお勧めします。

## 5. コードの生成と利用

1. **バックエンドの起動**: まず、`MyAspireApp.ApiService`プロジェクトを起動し、OpenAPI定義がアクセス可能であることを確認します。
2. **コードの生成**: フロントエンドプロジェクトのルートディレクトリで、以下のコマンドを実行します。

    ```bash
    cd MyAspireApp/src/myaspireapp-frontend
    npm run generate-api
    ```

    これにより、`./src/api`ディレクトリに型定義とAPIクライアントコードが生成されます。

3. **生成されたコードの利用**:
    生成されたAPIクライアントは、以下のようにインポートして使用できます。

    ```typescript
    // src/App.tsx (例)
    import React, { useEffect, useState } from 'react';
    import { WeatherForecast, DefaultApi } from './api'; // 生成された型とAPIクライアントをインポート

    function App() {
      const [weather, setWeather] = useState<WeatherForecast[]>([]);
      const [loading, setLoading] = useState(true);
      const [error, setError] = useState<string | null>(null);

      useEffect(() => {
        const fetchWeather = async () => {
          try {
            // DefaultApiは生成されたAPIクライアントのインスタンス
            const api = new DefaultApi();
            const data = await api.getWeatherForecast(); // 生成されたAPIメソッドを呼び出す
            setWeather(data);
          } catch (err) {
            setError('Failed to fetch weather data.');
            console.error(err);
          } finally {
            setLoading(false);
          }
        };

        fetchWeather();
      }, []);

      if (loading) return <div>Loading weather...</div>;
      if (error) return <div>Error: {error}</div>;

      return (
        <div>
          <h1>Weather Forecast</h1>
          <ul>
            {weather.map((forecast, index) => (
              <li key={index}>
                Date: {forecast.date}, Temp C: {forecast.temperatureC}, Summary: {forecast.summary}
              </li>
            ))}
          </ul>
        </div>
      );
    }

    export default App;
    ```

## 6. 自動化の検討

開発ワークフローに組み込むために、以下の自動化を検討できます。

* **プリコミットフック**: Gitのプリコミットフック (例: `husky`) を使用して、コミット前に`npm run generate-api`を実行し、常に最新の型定義がコミットされるようにする。
* **CI/CDパイプライン**: CI/CDパイプラインの一部として、バックエンドのデプロイ後やAPI定義の変更時に自動的にフロントエンドのコードを生成するステップを追加する。
