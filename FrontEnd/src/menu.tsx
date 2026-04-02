import * as React from 'react';
import AppBar from '@mui/material/AppBar';
import Box from '@mui/material/Box';
import CssBaseline from '@mui/material/CssBaseline';
import Divider from '@mui/material/Divider';
import Drawer from '@mui/material/Drawer';
import IconButton from '@mui/material/IconButton';
import List from '@mui/material/List';
import ListItem from '@mui/material/ListItem';
import ListItemButton from '@mui/material/ListItemButton';
import ListItemText from '@mui/material/ListItemText';
import MenuIcon from '@mui/icons-material/Menu';
import Toolbar from '@mui/material/Toolbar'
import ImgMediaCard from './profilecard';
import { Recommendations } from './recommendations';
import { Header } from './header';
import { BrowserRouter, Route, Routes, useNavigate, useLocation } from 'react-router-dom';
import { LikesPage } from './LikesPage';
import { Profile } from './Profile';
import { useAuth } from './AuthContext';

const drawerWidth = 240;

function MenuContent() {
  const [mobileOpen, setMobileOpen] = React.useState(false);
  const [isClosing, setIsClosing] = React.useState(false);
  const navigate = useNavigate();
  const { isLoggedIn, openDrawer, setOpenDrawer } = useAuth();

  React.useEffect(() => {
    if (!isLoggedIn) {
      navigate('/login');
    }
  }, [isLoggedIn, navigate]);

  React.useEffect(() => {
    if (openDrawer) {
      setMobileOpen(true);
      setOpenDrawer(false);
    }
  }, [openDrawer, setOpenDrawer]);

  const handleDrawerClose = () => {
    setIsClosing(true);
    setMobileOpen(false);
  };

  const handleDrawerTransitionEnd = () => {
    setIsClosing(false);
  };

  const handleDrawerToggle = () => {
    if (!isClosing) {
      setMobileOpen(!mobileOpen);
    }
  };

  const handleNavigation = (text: string) => {
    if (text === 'Home') {
      navigate('/');
    } else if (text === 'Likes') {
      navigate('/likes');
    } else if (text === 'Chat') {
      navigate('/chat');
    } else if (text === 'Liked you') {
      navigate('/likedyou');
    } else if (text === 'Profile') {
      navigate('/profile');
    }
  };

  const drawer = (
    <div>
      <Toolbar />
      <Divider />
      <List>
        {['Home', 'Likes', 'Chat', 'Liked you'].map((text) => (
          <ListItem key={text} disablePadding>
            <ListItemButton onClick={() => handleNavigation(text)}>
              <ListItemText primary={text} />
            </ListItemButton>
          </ListItem>
        ))}
      </List>
      <Divider />
      <List>
        {['Profile'].map((text, index) => (
          <ListItem key={text} disablePadding>
            <ListItemButton onClick={() => navigate('/profile')}>
              <ListItemText primary={text} />
            </ListItemButton>
          </ListItem>
        ))}
      </List>
    </div>
  );

  return (
    <Box sx={{ display: 'flex' }}>
      <CssBaseline />
      <Header onToggle={handleDrawerToggle}></Header>
      <Box
        component="nav"
        sx={{ width: { sm: drawerWidth }, flexShrink: { sm: 0 } }}
        aria-label="mailbox folders"
      >
        <Drawer
          variant="temporary"
          open={mobileOpen}
          onTransitionEnd={handleDrawerTransitionEnd}
          onClose={handleDrawerClose}
          sx={{
            display: { xs: 'block', sm: 'none' },
            '& .MuiDrawer-paper': { boxSizing: 'border-box', width: drawerWidth },
          }}
          slotProps={{
            root: {
              keepMounted: true,
            },
          }}
        >
          {drawer}
        </Drawer>
        <Drawer
          variant="permanent"
          sx={{
            display: { xs: 'none', sm: 'block' },
            '& .MuiDrawer-paper': { boxSizing: 'border-box', width: drawerWidth },
          }}
          open
        >
          {drawer}
        </Drawer>
      </Box>
      <Box
        component="main"
        sx={{ flexGrow: 1, p: 3, width: { sm: `calc(100% - ${drawerWidth}px)` } }}
      >
        <Toolbar />
        <Routes>
          <Route path="/" element={<Recommendations/>} />
          <Route path="/profile" element={<Profile/>} />
          <Route path="/likes" element={<LikesPage/>} />
          <Route path="/chat" element={<div>Chat</div>} />
          <Route path="/likedyou" element={<div>Liked you</div>} />
        </Routes>
      </Box>
    </Box>
  );
}

export default function Menu() {
  return <MenuContent />;
}
