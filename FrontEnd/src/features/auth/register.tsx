import { Box, Button, TextField, Typography, Paper, MenuItem } from '@mui/material';
import { AuthFooter } from './authFooter';
import { useRegisterForm } from '../../hooks/useRegisterForm';
import { AuthBranding } from './authBranding';

export function RegisterForm() {
    const { register, handleSubmit, watch, errors } = useRegisterForm();

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

                {/* Jobb oldali Register kártya */}
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
                            border: '1px solid #dbdbdb',
                            borderRadius: 3,
                            boxShadow: 'none',
                            backgroundColor: 'background.paper',
                            display: 'flex',
                            flexDirection: 'column',
                            maxHeight: '80vh',
                        }}
                    >
                        <Typography variant="h5" mb={1} sx={{ fontWeight: 600, color: 'text.primary' }} marginBottom={2} align="center">
                            Register
                        </Typography>

                        <AuthFooter
                            text="Already have an account?"
                            linkText="Sign in here!"
                            to="/login"
                        />

                        <Box
                            component="form"
                            onSubmit={handleSubmit}
                            noValidate
                            sx={{
                                overflowY: 'auto',
                                pr: 1,
                                my: 1,
                                '&::-webkit-scrollbar': { width: '6px' },
                                '&::-webkit-scrollbar-track': { backgroundColor: '#f1f1f1', borderRadius: '999px' },
                                '&::-webkit-scrollbar-thumb': { backgroundColor: '#c1c1c1', borderRadius: '999px' },
                            }}
                        >
                            <TextField
                                margin="normal" fullWidth label="Full Name"
                                {...register('fullName', { required: 'Full name is required!' })}
                                error={!!errors.fullName}
                                helperText={errors.fullName?.message as string}
                            />

                            <TextField
                                margin="normal" fullWidth label="Email" type="email"
                                {...register('email', { required: 'Email is required!' })}
                                error={!!errors.email}
                                helperText={errors.email?.message as string}
                            />

                            <TextField
                                margin="normal" fullWidth label="Date of Birth" type="date"
                                slotProps={{ inputLabel: { shrink: true } }}
                                {...register('birthDate', { required: 'Date of birth is required!' })}
                                error={!!errors.birthDate}
                                helperText={errors.birthDate?.message as string}
                            />

                            <TextField
                                margin="normal" fullWidth label="City"
                                {...register('city', { required: 'City is required!' })}
                                error={!!errors.city}
                                helperText={errors.city?.message as string}
                            />

                            <TextField
                                select margin="normal" fullWidth label="Gender" defaultValue=""
                                {...register('gender', { required: 'Gender is required!' })}
                                error={!!errors.gender}
                                helperText={errors.gender?.message as string}
                            >
                                <MenuItem value="female">Female</MenuItem>
                                <MenuItem value="male">Male</MenuItem>
                                <MenuItem value="other">Other</MenuItem>
                            </TextField>

                            <TextField
                                margin="normal" fullWidth label="Password" type="password"
                                {...register('password', { required: 'Password is required!' })}
                                error={!!errors.password}
                                helperText={errors.password?.message as string}
                            />

                            <TextField
                                margin="normal" fullWidth label="Confirm Password" type="password"
                                {...register('confirmPassword', {
                                    required: 'Confirm Password is required!',
                                    validate: (val: string) => watch('password') === val || "Passwords do not match!"
                                })}
                                error={!!errors.confirmPassword}
                                helperText={errors.confirmPassword?.message as string}
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
                                Create Account
                            </Button>
                        </Box>
                    </Paper>
                </Box>
            </Box>
        </Box>
    );
}