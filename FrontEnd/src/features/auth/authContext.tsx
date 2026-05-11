import * as React from 'react';
import { jwtDecode } from 'jwt-decode';

interface AuthContextType {
  isLoggedIn: boolean;
  token: string | null; 
  setIsLoggedIn: React.Dispatch<React.SetStateAction<boolean>>;
  login: (token: string, refreshToken: string) => void;
  logout: () => void;
  openDrawer: boolean;
  setOpenDrawer: React.Dispatch<React.SetStateAction<boolean>>;
}

const AuthContext = React.createContext<AuthContextType | undefined>(undefined);

export function AuthProvider({ children }: { children: React.ReactNode }) {
  // Állapot a tokennek
  const [token, setToken] = React.useState<string | null>(() => {
    if (typeof window === 'undefined') return null;
    return localStorage.getItem('token');
  });

  const [isLoggedIn, setIsLoggedIn] = React.useState(() => {
    if (typeof window === 'undefined' || !token) return false;

    try {
      const { exp } = jwtDecode<{ exp: number }>(token);
      if (Date.now() >= exp * 1000) {
        localStorage.removeItem('token');
        return false;
      }
      return true;
    } catch {
      return false;
    }
  });

  const [openDrawer, setOpenDrawer] = React.useState(false);

  const login = (newToken: string, newRefreshToken: string) => {
    localStorage.setItem('token', newToken);
    localStorage.setItem('refreshToken', newRefreshToken);
    setToken(newToken);
    setIsLoggedIn(true);
};

  const logout = React.useCallback(() => {
    localStorage.removeItem('token');
    localStorage.removeItem('refreshToken');
    setToken(null); // Töröljük a state-et
    setIsLoggedIn(false);
    setOpenDrawer(false);
  }, []);

  return (
    <AuthContext.Provider value={{ isLoggedIn, token, setIsLoggedIn, login, logout, openDrawer, setOpenDrawer }}>
      {children}
    </AuthContext.Provider>
  );
}

export function useAuth() {
  const context = React.useContext(AuthContext);
  if (!context) {
    throw new Error('useAuth must be used within AuthProvider');
  }
  return context;
}