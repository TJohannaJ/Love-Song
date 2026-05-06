import { Divider, List, ListItem, ListItemButton, ListItemText, Toolbar } from "@mui/material";
import React from "preact/compat";
import { useNavigate } from "react-router-dom";

export function MenuItems({ isLoggedIn }: { isLoggedIn: boolean }) {
    const navigate = useNavigate();
    
    React.useEffect(() => {
        if (!isLoggedIn) {
          navigate('/login');
        }
      }, [isLoggedIn, navigate]);

    const menuItems = [
    { text: 'Home', path: '/' },
    { text: 'Likes', path: '/likes' },
    { text: 'Chat', path: '/chat' },
  ];

  const drawer = (
    <div>
      <Toolbar />
      <Divider />
      <List>
        {menuItems.map((item) => (
          <ListItem key={item.text} disablePadding>
            <ListItemButton onClick={() => navigate(item.path)}>
              <ListItemText primary={item.text} />
            </ListItemButton>
          </ListItem>
        ))}
      </List>
      <Divider />
      <List>
        <ListItem disablePadding>
          <ListItemButton onClick={() => navigate('/profile')}>
            <ListItemText primary="Profile" />
          </ListItemButton>
        </ListItem>
      </List>
    </div>
  );
    return drawer;
}
