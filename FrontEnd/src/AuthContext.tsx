import * as React from 'react';

interface AuthContextType {
  isLoggedIn: boolean;
  setIsLoggedIn: React.Dispatch<React.SetStateAction<boolean>>;
  openDrawer: boolean;
  setOpenDrawer: React.Dispatch<React.SetStateAction<boolean>>;
}

const AuthContext = React.createContext<AuthContextType | undefined>(undefined);

export function AuthProvider({ children }: { children: React.ReactNode }) {
  const [isLoggedIn, setIsLoggedIn] = React.useState(() => {
    if (typeof window === 'undefined') return false;
    return Boolean(localStorage.getItem('token'));
  });
  const [openDrawer, setOpenDrawer] = React.useState(false);

  return (
    <AuthContext.Provider value={{ isLoggedIn, setIsLoggedIn, openDrawer, setOpenDrawer }}>
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
