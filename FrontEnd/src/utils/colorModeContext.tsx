import React, { createContext, useContext, useMemo, useState, useEffect } from 'react';
import { createTheme, ThemeProvider } from '@mui/material/styles';
import CssBaseline from '@mui/material/CssBaseline';

interface ColorModeContextType {
  toggleColorMode: () => void;
  mode: 'light' | 'dark';
}

const ColorModeContext = createContext<ColorModeContextType>({
  toggleColorMode: () => {},
  mode: 'light',
});

export const useColorMode = () => useContext(ColorModeContext);

export const ColorModeProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  // Kezdeti állapot kiolvasása a localStorage-ból (ha nincs elmentve, 'light' lesz az alapértelmezett)
  const [mode, setMode] = useState<'light' | 'dark'>(() => {
    const savedMode = localStorage.getItem('themeMode');
    return (savedMode === 'dark' || savedMode === 'light') ? savedMode : 'light';
  });

  // Amikor a mode változik, elmentjük a localStorage-ba
  useEffect(() => {
    localStorage.setItem('themeMode', mode);
  }, [mode]);

  const colorMode = useMemo(
    () => ({
      toggleColorMode: () => {
        setMode((prevMode) => (prevMode === 'light' ? 'dark' : 'light'));
      },
      mode,
    }),
    [mode]
  );

  const theme = useMemo(
    () =>
      createTheme({
        shape: {
          borderRadius: 12,
        },
        palette: {
          mode,
          primary: {
            light: '#FF5E00',
            main: '#FF007A',
            dark: '#C7005F',
            contrastText: '#ffffff',
          },
          ...(mode === 'light'
            ? {
                background: {
                  default: '#f1f2f4',
                  paper: '#ffffff',
                },
                text: {
                  primary: '#2f3035',
                  secondary: '#62636a',
                },
              }
            : {
                // Nagyon sötétkék háttér sötét mód esetén
                background: {
                  default: '#0a0e1a',
                  paper: '#121829',
                },
                text: {
                  primary: '#ffffff',
                  secondary: '#a0a7b8',
                },
              }),
        },
      }),
    [mode]
  );

  return (
    <ColorModeContext.Provider value={colorMode}>
      <ThemeProvider theme={theme}>
        <CssBaseline />
        {children}
      </ThemeProvider>
    </ColorModeContext.Provider>
  );
};