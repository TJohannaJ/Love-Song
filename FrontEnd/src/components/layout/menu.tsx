import * as React from 'react';
import Box from '@mui/material/Box';
import CssBaseline from '@mui/material/CssBaseline';
import Drawer from '@mui/material/Drawer';
import Toolbar from '@mui/material/Toolbar';
import { Recommendations } from '../../features/recommendation/recommendationsPage';
import { Header } from './header';
import { Route, Routes, useLocation } from 'react-router-dom';
import { useAuth } from '../../features/auth/authContext';
import { MenuItems } from '../../utils/menuItems';
import { ProfilePage } from '../../features/profile/profilePage';
import { ChatPage } from '../../features/chat/chatPage';
import { LikesPage } from '../../features/recommendation/likesPage';

const drawerWidth = 240;

export function Menu() {
  const [mobileOpen, setMobileOpen] = React.useState(false);
  const [isClosing, setIsClosing] = React.useState(false);
  const { isLoggedIn, openDrawer, setOpenDrawer } = useAuth();

  const location = useLocation();
  const isChatRoute = location.pathname === '/chat';

  React.useEffect(() => {
    if (openDrawer) {
      setMobileOpen(true);
      setOpenDrawer(false);
    }
  }, [openDrawer, setOpenDrawer]);

  return (
    <Box sx={{ display: 'flex' }}>
      <CssBaseline />
      <Header isChatRoute={isChatRoute} onToggle=
        {() => {
          if (!isClosing) {
            setMobileOpen(!mobileOpen);
          }
        }} />

      <Box
        component="nav"
        sx={{
          width: { sm: isChatRoute ? 0 : drawerWidth },
          flexShrink: { sm: 0 }
        }}
      >
        <Drawer
          variant="temporary"
          open={mobileOpen}
          onTransitionEnd={() => setIsClosing(false)}
          onClose={() => { setIsClosing(true); setMobileOpen(false); }}
          sx={{
            display: { xs: 'block', sm: isChatRoute ? 'block' : 'none' },
            '& .MuiDrawer-paper': { boxSizing: 'border-box', width: drawerWidth },
          }}
          slotProps={{ root: { keepMounted: true } }}
        >
          <MenuItems isLoggedIn={isLoggedIn} />
        </Drawer>

        <Drawer
          variant="permanent"
          sx={{
            display: { xs: 'none', sm: isChatRoute ? 'none' : 'block' },
            '& .MuiDrawer-paper': { boxSizing: 'border-box', width: drawerWidth },
          }}
          open
        >
          <MenuItems isLoggedIn={isLoggedIn} />
        </Drawer>
      </Box>

      <Box
        component="main"
        sx={{
          flexGrow: 1,
          p: 3,
          width: { sm: isChatRoute ? '100%' : `calc(100% - ${drawerWidth}px)` },
          minHeight: '100vh'
        }}
      >
        <Toolbar />
        <Routes>
          <Route path="/" element={<Recommendations />} />
          <Route path="/profile" element={<ProfilePage />} />
          <Route path="/likes" element={<LikesPage />} />
          <Route path="/chat" element={<ChatPage />} />
        </Routes>
      </Box>
    </Box>
  );
}