import { defineConfig } from 'vite';
import preact from '@preact/preset-vite';
import process from 'process';
import path from 'path';
import fs from 'fs';
import child_process from 'child_process';
import { fileURLToPath } from 'url';

const env = process.env;

// ASP.NET HTTPS tanúsítványok helyének meghatározása
const baseFolder =
    env.APPDATA !== undefined && env.APPDATA !== ''
        ? `${env.APPDATA}/ASP.NET/https`
        : `${env.HOME}/.aspnet/https`;

const certificateName = "reactapp1.client";
const certFilePath = path.join(baseFolder, `${certificateName}.pem`);
const keyFilePath = path.join(baseFolder, `${certificateName}.key`);

// Mappa létrehozása, ha nem létezik
if (!fs.existsSync(baseFolder)) {
    fs.mkdirSync(baseFolder, { recursive: true });
}

// Tanúsítvány generálása, ha hiányzik (dotnet dev-certs használatával)
if (!fs.existsSync(certFilePath) || !fs.existsSync(keyFilePath)) {
    if (0 !== child_process.spawnSync('dotnet', [
        'dev-certs',
        'https',
        '--export-path',
        certFilePath,
        '--format',
        'Pem',
        '--no-password',
    ], { stdio: 'inherit', }).status) {
        throw new Error("Could not create certificate.");
    }
}

// A backend címe - a 127.0.0.1-et használjuk a localhost helyett a stabilitás miatt
const target = 'https://127.0.0.1:7160';

export default defineConfig({
    plugins: [preact()],
    resolve: {
        alias: {
            // A '@' szimbólum mostantól a 'src' mappára mutat
            '@': fileURLToPath(new URL('./src', import.meta.url))
        }
    },
    server: {
        proxy: {
            // Minden /api-val kezdődő kérést átirányítunk a backendre
            '^/api': {
                target: target,
                secure: false,
                changeOrigin: true
            }
        },
        port: 5175,
        strictPort: true,
        https: {
            key: fs.readFileSync(keyFilePath),
            cert: fs.readFileSync(certFilePath),
        }
    },
    // Production preview beállítások
    preview: {
        port: 5175,
        strictPort: true,
    }
});