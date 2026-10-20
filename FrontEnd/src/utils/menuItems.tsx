import { Box, Divider, List, ListItem, ListItemButton, ListItemIcon, ListItemText, Toolbar } from "@mui/material";
import HomeOutlinedIcon from '@mui/icons-material/HomeOutlined';
import FavoriteBorderIcon from '@mui/icons-material/FavoriteBorder';
import ChatOutlinedIcon from '@mui/icons-material/ChatOutlined';
import PersonOutlineIcon from '@mui/icons-material/PersonOutline';
import DarkModeOutlinedIcon from '@mui/icons-material/DarkModeOutlined';
import LightModeOutlinedIcon from '@mui/icons-material/LightModeOutlined';
import AutoAwesomeIcon from '@mui/icons-material/AutoAwesome'; // <-- 1. ÚJ IMPORT
import React from "preact/compat";
import { useLocation, useNavigate } from "react-router-dom";
import logo from '../assets/logo.png';
import { GradientIcon } from "../components/common/gradientIcon";
import { useColorMode } from '../utils/colorModeContext';

export function MenuItems({ isLoggedIn, expanded = false }: { isLoggedIn: boolean; expanded?: boolean }) {
    const navigate = useNavigate();
    const location = useLocation();
    const { toggleColorMode, mode } = useColorMode();

    const isDarkMode = mode === 'dark';

    const navigateHomeOnDesktop = () => {
        if (window.matchMedia('(min-width: 600px)').matches) {
            navigate('/');
        }
    };

    const handleBrandKeyDown = (event: React.KeyboardEvent<HTMLElement>) => {
        if ((event.key === 'Enter' || event.key === ' ') && window.matchMedia('(min-width: 600px)').matches) {
            event.preventDefault();
            navigate('/');
        }
    };

    React.useEffect(() => {
        if (!isLoggedIn) {
            navigate('/login');
        }
    }, [isLoggedIn, navigate]);

    // 2. ÚJ MENÜPONT HOZZÁADÁSA A TÖMBHÖZ:
    const menuItems = [
        { text: 'Home', path: '/', icon: <HomeOutlinedIcon /> },
        { text: 'Likes', path: '/likes', icon: <FavoriteBorderIcon /> },
        { text: 'Chat', path: '/chat', icon: <ChatOutlinedIcon /> },
        { text: 'AI Assistant', path: '/ai-chat', icon: <AutoAwesomeIcon /> }, // <-- ÚJ MENÜPONT
    ];

    return (
        <Box sx={{ display: 'flex', flexDirection: 'column', height: '100%' }}>
            {/* Felső navigációs rész */}
            <Box sx={{ flexGrow: 1 }}>
                <Toolbar
                    disableGutters
                    onClick={navigateHomeOnDesktop}
                    onKeyDown={handleBrandKeyDown}
                    role="button"
                    tabIndex={0}
                    sx={{
                        background: 'linear-gradient(135deg, #FF007A 0%, #FF5E00 100%)',
                        display: 'flex',
                        alignItems: 'center',
                        justifyContent: 'flex-start',
                        px: 1.5,
                        minHeight: '64px !important',
                        overflow: 'hidden',
                        width: '100%',
                        cursor: { xs: 'default', sm: 'pointer' },
                        outline: 'none',
                        transition: 'background-color 0.2s ease',
                        '&:focus-visible': {
                            boxShadow: 'inset 0 0 0 2px rgba(255,255,255,0.9)',
                        },
                    }}
                >
                    <Box
                        component="img"
                        src={logo}
                        alt="LoveSong logo"
                        sx={{
                            height: 40,
                            width: 40,
                            objectFit: 'contain',
                            filter: 'brightness(0) invert(1)',
                            flexShrink: 0,
                            pointerEvents: 'none',
                        }}
                    />

                    {expanded && (
                        <Box
                            component="span"
                            sx={{
                                color: '#ffffff',
                                fontFamily: 'Tangerine, cursive !important',
                                fontSize: '40px',
                                fontWeight: 'bold',
                                lineHeight: '1',
                                whiteSpace: 'nowrap',
                                textShadow: '0 1px 3px rgba(0, 0, 0, 0.35)',
                                ml: 1.5,
                                pointerEvents: 'none',
                            }}
                            marginTop={2}
                        >
                            LoveSong
                        </Box>
                    )}
                </Toolbar>
                <Divider />
                <List>
                    {menuItems.map((item) => {
                        const isActive = location.pathname === item.path;
                        return (
                            <ListItem key={item.text} disablePadding>
                                <ListItemButton
                                    onClick={() => navigate(item.path)}
                                    selected={isActive}
                                    sx={{
                                        '&.Mui-selected': {
                                            backgroundColor: 'rgba(255, 0, 122, 0.08)',
                                            '&:hover': {
                                                backgroundColor: 'rgba(255, 0, 122, 0.12)',
                                            },
                                        },
                                    }}
                                >
                                    <ListItemIcon sx={{ minWidth: 0, mr: 2 }}>
                                        <GradientIcon>{item.icon}</GradientIcon>
                                    </ListItemIcon>
                                    <ListItemText
                                        primary={item.text}
                                        sx={{
                                            opacity: expanded ? 1 : 0,
                                            whiteSpace: 'nowrap',
                                            transition: 'opacity 0.2s ease',
                                            '& .MuiTypography-root': {
                                                fontWeight: isActive ? 600 : 400,
                                            },
                                        }}
                                    />
                                </ListItemButton>
                            </ListItem>
                        );
                    })}
                </List>
                <Divider />
                <List>
                    <ListItem disablePadding>
                        <ListItemButton
                            onClick={() => navigate('/profile')}
                            selected={location.pathname === '/profile'}
                            sx={{
                                '&.Mui-selected': {
                                    backgroundColor: 'rgba(255, 0, 122, 0.08)',
                                    '&:hover': {
                                        backgroundColor: 'rgba(255, 0, 122, 0.12)',
                                    },
                                },
                            }}
                        >
                            <ListItemIcon sx={{ minWidth: 0, mr: 2 }}>
                                <GradientIcon>
                                    <PersonOutlineIcon />
                                </GradientIcon>
                            </ListItemIcon>
                            <ListItemText
                                primary="Profile"
                                sx={{
                                    opacity: expanded ? 1 : 0,
                                    whiteSpace: 'nowrap',
                                    transition: 'opacity 0.2s ease',
                                    '& .MuiTypography-root': {
                                        fontWeight: location.pathname === '/profile' ? 600 : 400,
                                    },
                                }}
                            />
                        </ListItemButton>
                    </ListItem>
                </List>
            </Box>

            {/* Menü alján elhelyezett sötét mód gomb */}
            <Box sx={{ pb: 1 }}>
                <Divider />
                <List>
                    <ListItem disablePadding>
                        <ListItemButton onClick={toggleColorMode}>
                            <ListItemIcon sx={{ minWidth: 0, mr: 2 }}>
                                <GradientIcon>
                                    {isDarkMode ? <LightModeOutlinedIcon /> : <DarkModeOutlinedIcon />}
                                </GradientIcon>
                            </ListItemIcon>
                            <ListItemText
                                primary={isDarkMode ? 'Light mode' : 'Dark mode'}
                                sx={{
                                    opacity: expanded ? 1 : 0,
                                    whiteSpace: 'nowrap',
                                    transition: 'opacity 0.2s ease',
                                }}
                            />
                        </ListItemButton>
                    </ListItem>
                </List>
            </Box>
        </Box>
    );
}