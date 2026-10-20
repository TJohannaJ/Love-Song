import { Box, Button, TextField, Typography, Paper } from '@mui/material';
import { useLoginForm } from '../../hooks/useLoginForm';
import { AuthFooter } from './authFooter';
import { AuthBranding } from './authBranding';

export function LoginForm() {
    const { register, handleSubmit, errors } = useLoginForm();

    return (
        <Box
            sx={{
                height: '100vh',
                width: '100vw',
                boxSizing: 'border-box',
                overflow: 'hidden',
                display: 'flex',
                alignItems: 'center',
                justifyContent: 'center',
                backgroundColor: 'background.default',
                px: { xs: 2, md: 6 },
                py: { xs: 3, md: 5 },
            }}
        >
            <Box
                sx={{
                    width: '100%',
                    maxWidth: 1200,
                    display: 'grid',
                    gridTemplateColumns: { xs: '1fr', md: '1.2fr 0.8fr' },
                    alignItems: 'center',
                    gap: { xs: 2, md: 4 },
                }}
            >
                {/* Közös Bal Oldal */}
                <AuthBranding />

                {/* Jobb oldali Login kártya */}
                <Box
                    sx={{
                        display: 'flex',
                        alignItems: 'center',
                        justifyContent: 'center',
                        px: { xs: 0, md: 2 },
                    }}
                >
                    <Paper
                        sx={{
                            p: 3,
                            width: '100%',
                            maxWidth: 420,
                            border: '1px solid',
                            borderColor: 'divider',
                            borderRadius: 3,
                            boxShadow: 'none',
                            backgroundColor: 'background.paper',
                        }}
                    >
                        <Typography variant="h5" mb={1} sx={{ fontWeight: 600, color: 'text.primary' }} marginBottom={2} align="center">
                            Login
                        </Typography>

                        <AuthFooter
                            text="Don't have an account yet?"
                            linkText="Register here!"
                            to="/register"
                        />

                        <Box component="form" onSubmit={handleSubmit} noValidate sx={{ my: 1 }}>
                            <TextField
                                margin="normal"
                                fullWidth
                                label="Email"
                                {...register('email', { required: 'Email is required!' })}
                                error={!!errors.email}
                                helperText={errors.email?.message as string}
                            />

                            <TextField
                                margin="normal"
                                fullWidth
                                label="Password"
                                type="password"
                                {...register('password', { required: 'Password is required!' })}
                                error={!!errors.password}
                                helperText={errors.password?.message as string}
                            />

                            <Button
                                type="submit"
                                fullWidth
                                variant="contained"
                                sx={{
                                    mt: 3,
                                    mb: 1,
                                    py: 1.2,
                                    fontWeight: 600,
                                    background: 'linear-gradient(135deg, #FF007A 0%, #FF5E00 100%)',
                                    color: '#ffffff',
                                    boxShadow: '0 4px 12px rgba(255, 0, 122, 0.3)',
                                    transition: 'all 0.2s ease-in-out',
                                    '&:hover': {
                                        background: 'linear-gradient(135deg, #E0006C 0%, #E65300 100%)',
                                        boxShadow: '0 6px 16px rgba(255, 0, 122, 0.4)',
                                    }
                                }}
                            >
                                Login
                            </Button>
                        </Box>
                    </Paper>
                </Box>
            </Box>
        </Box>
    );
}