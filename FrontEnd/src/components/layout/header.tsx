import * as React from 'react';
import AppBar from '@mui/material/AppBar';
import Toolbar from '@mui/material/Toolbar';
import IconButton from '@mui/material/IconButton';
import MenuIcon from '@mui/icons-material/Menu';
import { Box, Typography } from '@mui/material';
import logo from '../../assets/logo.png'; // Igazítsd az útvonalat, ha szükséges!

interface HeaderProps {
  onToggle: () => void;
}

export const Header = ({ onToggle }: HeaderProps) => {
  return (
    <AppBar
      position="fixed"
      sx={{
        display: { 
          xs: 'block',
          sm: 'none'
        },
        background: 'linear-gradient(135deg, #FF007A 0%, #FF5E00 100%)',
        boxShadow: '0 4px 15px rgba(255, 0, 122, 0.2)',
      }}
    >
      <Toolbar>
        {/* Hamburger menü ikon */}
        <IconButton
          color="inherit"
          aria-label="open drawer"
          edge="start"
          onClick={onToggle}
          sx={{ mr: 2, color: '#ffffff' }}
        >
          <MenuIcon />
        </IconButton>

        {/* Logó és Cím - Középre igazítva */}
        <Box 
          sx={{ 
            flexGrow: 1, 
            display: 'flex', 
            justifyContent: 'center', 
            alignItems: 'center',
            pr: 5, // Ezzel kompenzáljuk a bal oldali menügomb szélességét, hogy tényleg középen legyen
          }}
        >
          <Box
            component="img"
            src={logo}
            alt="LoveSong logo"
            sx={{
              height: 36,
              width: 36,
              objectFit: 'contain',
              filter: 'brightness(0) invert(1)',
              pointerEvents: 'none',
            }}
          />
          <Typography
            sx={{
              color: '#ffffff',
              fontFamily: 'Tangerine, cursive',
              fontSize: '36px',
              fontWeight: 'bold',
              lineHeight: 1,
              ml: 1.5,
              mt: 0.5,
              textShadow: '0 1px 3px rgba(0, 0, 0, 0.25)',
              pointerEvents: 'none',
            }}
          >
            LoveSong
          </Typography>
        </Box>
      </Toolbar>
    </AppBar>
  );
};