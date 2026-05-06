import ReactDOM from 'react-dom/client';
import { RegisterForm } from './features/auth/register';
import { BrowserRouter, Navigate, Route, Routes } from 'react-router-dom';
import { AuthProvider, useAuth } from './features/auth/authContext';
import React from 'react';
import { Menu } from './components/layout/menu';
import { LoginForm } from './features/auth/login';

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
        <Route path="/*" element={
            <RequireAuth>
              <Menu />
            </RequireAuth>
          } />
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
    <App />
  </React.StrictMode>
);
