import * as React from 'react';
import AppBar from '@mui/material/AppBar';
import Toolbar from '@mui/material/Toolbar';
import IconButton from '@mui/material/IconButton';
import MenuIcon from '@mui/icons-material/Menu';

interface HeaderProps {
  onToggle: () => void;
  isChatRoute: boolean; 
}

export const Header = ({ onToggle, isChatRoute }: HeaderProps) => { // <-- Itt is vedd fel!
  return (
    <AppBar
      position="fixed"
      sx={{
        display: { 
          xs: 'block', 
          sm: isChatRoute ? 'block' : 'none' 
        },
      }}
    >
      <Toolbar>
        <IconButton
          color="inherit"
          aria-label="open drawer"
          edge="start"
          onClick={onToggle}
          sx={{ mr: 2 }}
        >
          <MenuIcon />
        </IconButton>
      </Toolbar>
    </AppBar>
  );
};