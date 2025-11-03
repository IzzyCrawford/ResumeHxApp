import { fileURLToPath, URL } from 'node:url';
import { defineConfig } from 'vite';
import plugin from '@vitejs/plugin-react';

export default defineConfig({
    plugins: [plugin()],
    resolve: {
        alias: {
            '@': fileURLToPath(new URL('./src', import.meta.url))
        }
    },
    server: {
        port: 3000,
        proxy: {
            '/weatherforecast': {
                target: process.env.VITE_API_HOST || 'http://localhost:5000',
                changeOrigin: true,
                secure: false
            }
        }
    }
});