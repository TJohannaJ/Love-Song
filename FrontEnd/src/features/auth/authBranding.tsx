import { Box, Typography } from '@mui/material';
import logo from '../../assets/logo.png';

export function AuthBranding() {
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
                {/* Csak az ikon kapott lekerekített, gradienses hátteret */}
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

                {/* A név külön, tiszta szövegként */}
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
        </Box>
    );
}