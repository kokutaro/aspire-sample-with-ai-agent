/// <reference types="vitest" />
import react from '@vitejs/plugin-react'
import { defineConfig } from 'vite'

// https://vitejs.dev/config/
export default defineConfig({
  plugins: [react()],
  test: {
    globals: true,
    environment: 'jsdom',
    setupFiles: './src/setupTests.ts',
    coverage: {
      reporter: ['text', 'json', 'html', 'lcov'],
      exclude: [
        '**/*.config.ts',
        '**/*.config.js',
        'package.json',
        'package-lock.json',
        'tsconfig.json',
        'tsconfig.app.json',
        'tsconfig.node.json',
        'node_modules/**',
        '**/__tests__/**',
        'src/vite-env.d.ts',
        'src/main.tsx',
      ],
    },
  },
})
