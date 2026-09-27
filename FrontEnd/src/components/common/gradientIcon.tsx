import React from 'react';
import { Box } from '@mui/material';

export function GradientIcon({ children }: { children: React.ReactNode }) {
  return (
    <Box
      sx={{
        display: 'inline-flex',
        alignItems: 'center',
        justifyContent: 'center',
        // 1. Gradiens háttér beállítása
        background: 'linear-gradient(135deg, #FF007A 0%, #FF5E00 100%)',
        // 2. Gradiens maszkolása a szövegre/ikonra
        WebkitBackgroundClip: 'text',
        WebkitTextFillColor: 'transparent',
        // 3. Biztosítjuk, hogy az SVG ikon átvegye ezt a színátmenetet
        '& .MuiSvgIcon-root': {
          fontSize: 30,
          fill: 'currentColor',
          color: '#FF007A', // Fallback, ha a background-clip nem támogatott
        },
        // 4. Felülírjuk az összes belső SVG path alapértelmezett színét
        '& svg path': {
          fill: 'currentColor !important',
        },
      }}
    >
      {children}
    </Box>
  );
}