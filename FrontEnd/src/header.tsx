import * as React from 'react';
import AppBar from '@mui/material/AppBar';
import Toolbar from '@mui/material/Toolbar';
import IconButton from '@mui/material/IconButton';
import MenuIcon from '@mui/icons-material/Menu';

interface HeaderProps {
  onToggle: () => void;
}

export const Header = ({ onToggle }: HeaderProps) => {
  return (
    <AppBar
      position="fixed"
      sx={{
        display: { xs: 'block', sm: 'none' }, 
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