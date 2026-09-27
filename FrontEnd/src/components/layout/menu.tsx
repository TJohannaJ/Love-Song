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
import { LikesPage } from '../../features/likes/likesPage';

const drawerWidth = 240;
const collapsedDrawerWidth = 64;

export function Menu() {
  const [mobileOpen, setMobileOpen] = React.useState(false);
  const [isClosing, setIsClosing] = React.useState(false);
  const [desktopDrawerExpanded, setDesktopDrawerExpanded] = React.useState(false);
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
      <Header onToggle=
        {() => {
          if (!isClosing) {
            setMobileOpen(!mobileOpen);
          }
        }} />

      <Box
        component="nav"
        onMouseEnter={() => setDesktopDrawerExpanded(true)}
        onMouseLeave={() => setDesktopDrawerExpanded(false)}
        sx={{
          width: { sm: desktopDrawerExpanded ? drawerWidth : collapsedDrawerWidth },
          flexShrink: { sm: 0 },
          transition: 'width 0.3s ease',
        }}
      >
        <Drawer
          variant="temporary"
          open={mobileOpen}
          onTransitionEnd={() => setIsClosing(false)}
          onClose={() => { setIsClosing(true); setMobileOpen(false); }}
          sx={{
            display: { xs: 'block', sm: 'none' },
            '& .MuiDrawer-paper': { boxSizing: 'border-box', width: drawerWidth },
          }}
          slotProps={{ root: { keepMounted: true } }}
        >
          <MenuItems isLoggedIn={isLoggedIn} expanded />
        </Drawer>

        <Drawer
          variant="permanent"
          sx={{
            display: { xs: 'none', sm: 'block' },
            '& .MuiDrawer-paper': {
              boxSizing: 'border-box',
              width: desktopDrawerExpanded ? drawerWidth : collapsedDrawerWidth,
              overflowX: 'hidden',
              transition: 'width 0.3s ease',
            },
          }}
          open
        >
          <MenuItems isLoggedIn={isLoggedIn} expanded={desktopDrawerExpanded} />
        </Drawer>
      </Box>

      <Box
        component="main"
        sx={{
          flexGrow: 1,
          p: 3,
          width: { sm: `calc(100% - ${collapsedDrawerWidth}px)` },
          minHeight: '100vh',
          transition: 'width 0.3s ease',
          ...(desktopDrawerExpanded && {
            width: { sm: `calc(100% - ${drawerWidth}px)` },
          }),
        }}
      >
        <Toolbar sx={{ display: { xs: 'block', sm: 'none' } }} />
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