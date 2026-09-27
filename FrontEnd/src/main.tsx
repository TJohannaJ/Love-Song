import React from 'react';
import ReactDOM from 'react-dom/client';
import { BrowserRouter, Navigate, Route, Routes } from 'react-router-dom';
import { createTheme, ThemeProvider } from '@mui/material/styles';
import CssBaseline from '@mui/material/CssBaseline';

import { RegisterForm } from './features/auth/register';
import { LoginForm } from './features/auth/login';
import { AuthProvider, useAuth } from './features/auth/authContext';
import { Menu } from './components/layout/menu';

const theme = createTheme({
    shape: {
        borderRadius: 12,
    },
    palette: {
        mode: 'light',
        primary: {
            light: '#FF5E00',
            main: '#FF007A',
            dark: '#C7005F',
            contrastText: '#ffffff',
        },
        background: {
            default: '#f1f2f4', // Egységes szürke alapháttér
            paper: '#ffffff',
        },
        text: {
            primary: '#2f3035',
            secondary: '#62636a',
        },
    },
});

function RequireAuth({ children }: { children: React.ReactNode }) {
    const { isLoggedIn } = useAuth();
    if (!isLoggedIn) {
        return <Navigate to="/login" replace />;
    }
    return <>{children}</>;
}

export function App() {
    return (
        <AuthProvider>
            <BrowserRouter>
                <Routes>
                    <Route path="/login" element={<LoginForm />} />
                    <Route path="/register" element={<RegisterForm />} />
                    <Route
                        path="/*"
                        element={
                            <RequireAuth>
                                <Menu />
                            </RequireAuth>
                        }
                    />
                </Routes>
            </BrowserRouter>
        </AuthProvider>
    );
}

const rootElement = document.getElementById('root') || document.getElementById('app');

if (!rootElement) {
    throw new Error("Nem található a 'root' vagy 'app' elem az index.html-ben!");
}

const root = ReactDOM.createRoot(rootElement);
root.render(
    <React.StrictMode>
        <ThemeProvider theme={theme}>
            <CssBaseline />
            <App />
        </ThemeProvider>
    </React.StrictMode>
);