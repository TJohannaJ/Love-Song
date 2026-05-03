import * as React from 'react';
import Box from '@mui/material/Box';
import CssBaseline from '@mui/material/CssBaseline';
import Drawer from '@mui/material/Drawer';
import Toolbar from '@mui/material/Toolbar';
import { Recommendations } from './Pages/recommendationsPage';
import { Header } from './header';
import { Route, Routes, useLocation } from 'react-router-dom';
import { useAuth } from './LoginPage/authContext';
import { MenuItems } from './menuItems';
import { ProfilePage } from './Pages/profilePage';
import { LikesPage } from './Pages/likesPage';
import { ChatPage } from './Pages/chatPage';

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
        {/* TEMPORARY DRAWER - Ez felel a mobilos viselkedésért */}
        <Drawer
          variant="temporary"
          open={mobileOpen}
          onTransitionEnd={() => setIsClosing(false)}
          onClose={() => { setIsClosing(true); setMobileOpen(false); }}
          sx={{
            // Ha chaten vagyunk, sm méret felett is maradjon block (látható, ha open)
            display: { xs: 'block', sm: isChatRoute ? 'block' : 'none' },
            '& .MuiDrawer-paper': { boxSizing: 'border-box', width: drawerWidth },
          }}
          slotProps={{ root: { keepMounted: true } }}
        >
          <MenuItems isLoggedIn={isLoggedIn} />
        </Drawer>

        {/* PERMANENT DRAWER - Az állandó oldalsáv */}
        <Drawer
          variant="permanent"
          sx={{
            // Ha chaten vagyunk, sm méret felett is tűnjön el (none)
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
          // Dinamikus szélesség: ha chat, akkor 100%, különben sm felett levonjuk a menüt
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