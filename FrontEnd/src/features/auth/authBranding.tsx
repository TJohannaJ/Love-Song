import { Box, Typography, IconButton, Tooltip } from '@mui/material';
import DarkModeOutlinedIcon from '@mui/icons-material/DarkModeOutlined';
import LightModeOutlinedIcon from '@mui/icons-material/LightModeOutlined';
import logo from '../../assets/logo.png';
import { useColorMode } from '../../utils/colorModeContext';

export function AuthBranding() {
    const { toggleColorMode, mode } = useColorMode();
    const isDarkMode = mode === 'dark';

    return (
        <Box
            sx={{
                display: 'flex',
                flexDirection: 'column',
                justifyContent: 'center',
                alignItems: 'center',
                textAlign: 'center',
                px: { xs: 0, md: 2 },
                pt: { xs: 0, md: 0 },
                pb: { xs: 2, md: 0 },
                backgroundColor: 'transparent',
                position: 'relative',
            }}
        >
            {/* Logó és Cím egymás mellett */}
            <Box
                sx={{
                    display: 'flex',
                    flexDirection: 'row',
                    alignItems: 'center',
                    mb: 3,
                }}
            >
                <Box
                    sx={{
                        background: 'linear-gradient(135deg, #FF007A 0%, #FF5E00 100%)',
                        alignItems: 'center',
                        justifyContent: 'center',
                        p: { xs: 1.5, md: 2 },
                        borderRadius: '20px',
                        display: 'inline-flex',
                    }}
                >
                    <Box
                        component="img"
                        src={logo}
                        alt="LoveSong logo"
                        sx={{
                            width: { xs: 45, md: 60 },
                            height: { xs: 45, md: 60 },
                            objectFit: 'contain',
                            filter: 'brightness(0) invert(1)',
                            pointerEvents: 'none',
                        }}
                    />
                </Box>

                <Typography
                    variant="h2"
                    sx={{
                        fontWeight: 700,
                        letterSpacing: '-0.03em',
                        lineHeight: 1,
                        color: 'text.primary',
                        fontSize: { xs: '3.5rem', md: '5.5rem' },
                        fontFamily: 'Tangerine, cursive',
                        ml: 2.5,
                        mt: 1,
                    }}
                >
                    LoveSong
                </Typography>
            </Box>

            <Typography
                variant="h6"
                sx={{
                    maxWidth: 450,
                    color: 'text.secondary',
                    fontWeight: 500,
                    fontSize: { xs: '1.1rem', md: '1.4rem' },
                    lineHeight: 1.4,
                    fontStyle: 'italic',
                }}
            >
                Find your love through your favourite songs!
            </Typography>

            {/* Bal alsó sarokban lévő gomb */}
            <Tooltip title={isDarkMode ? 'Világos mód' : 'Sötét mód'}>
                <IconButton
                    onClick={toggleColorMode}
                    sx={{
                        position: 'fixed',
                        bottom: 20,
                        left: 20,
                        backgroundColor: 'background.paper',
                        color: 'text.primary',
                        border: '1px solid',
                        borderColor: 'divider',
                        boxShadow: '0 4px 12px rgba(0,0,0,0.1)',
                        '&:hover': {
                            backgroundColor: 'background.paper',
                            boxShadow: '0 6px 16px rgba(0,0,0,0.15)',
                        },
                    }}
                >
                    {isDarkMode ? <LightModeOutlinedIcon fontSize="small" /> : <DarkModeOutlinedIcon fontSize="small" />}
                </IconButton>
            </Tooltip>
        </Box>
    );
}