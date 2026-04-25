import * as React from 'react';
import Box from '@mui/material/Box';
import CssBaseline from '@mui/material/CssBaseline';
import Drawer from '@mui/material/Drawer';
import Toolbar from '@mui/material/Toolbar';
import { Recommendations } from './Pages/recommendationsPage';
import { Header } from './header';
import { Route, Routes } from 'react-router-dom';
import { useAuth } from './LoginPage/authContext';
import { MenuItems } from './menuItems';
import { ProfilePage } from './Pages/profilePage';
import { LikesPage } from './Pages/likesPage';

const drawerWidth = 240;

export function Menu() {
  const [mobileOpen, setMobileOpen] = React.useState(false);
  const [isClosing, setIsClosing] = React.useState(false);  
  const { isLoggedIn, openDrawer, setOpenDrawer } = useAuth();

  React.useEffect(() => {
    if (openDrawer) {
      setMobileOpen(true);
      setOpenDrawer(false);
    }
  }, [openDrawer, setOpenDrawer]);

  return (
    <Box sx={{ display: 'flex' }}>
      <CssBaseline />
      <Header onToggle={() => {
        if (!isClosing) {
          setMobileOpen(!mobileOpen);
        }
      }}></Header>
      <Box component="nav" sx={{ width: { sm: drawerWidth }, flexShrink: { sm: 0 } }} aria-label="mailbox folders">
        <Drawer
          variant="temporary"
          open={mobileOpen}
          onTransitionEnd={() => setIsClosing(false)}
          onClose={() => { setIsClosing(true); setMobileOpen(false); }}
          sx={{ display: { xs: 'block', sm: 'none' }, '& .MuiDrawer-paper': { boxSizing: 'border-box', width: drawerWidth }, }}
          slotProps={{ root: { keepMounted: true, }, }}
        >
          <MenuItems isLoggedIn={isLoggedIn} />
        </Drawer>
        <Drawer
          variant="permanent"
          sx={{ display: { xs: 'none', sm: 'block' }, '& .MuiDrawer-paper': { boxSizing: 'border-box', width: drawerWidth }, }}
          open
        >
          <MenuItems isLoggedIn={isLoggedIn} />
        </Drawer>
      </Box>
      <Box component="main" sx={{ flexGrow: 1, p: 3, width: { sm: `calc(100% - ${drawerWidth}px)` } }}>
        <Toolbar/>
        <Routes>
          <Route path="/" element={<Recommendations />} />
          <Route path="/profile" element={<ProfilePage/>} />
          <Route path="/likes" element={<LikesPage />} />
          <Route path="/chat" element={<div>Chat</div>} />
        </Routes>
      </Box>
    </Box>
  );
}
