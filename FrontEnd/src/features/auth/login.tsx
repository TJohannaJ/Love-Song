import { Box, Button, TextField, Typography, Paper } from '@mui/material';
import { useLoginForm } from '../../hooks/useLoginForm';
import { AuthFooter } from './authFooter';

export function LoginForm() {
    const { register, handleSubmit, errors } = useLoginForm();

    return (
        <Paper sx={{ p: 4, maxWidth: 400, mx: 'auto', mt: 5 }}>
            <Typography variant="h5" mb={2}>Bejelentkezés</Typography>

            <AuthFooter 
                text="Nincs még fiókod?" 
                linkText="Regisztrálj itt!" 
                to="/register" 
            />

            <Box component="form" onSubmit={handleSubmit} noValidate>
                <TextField
                    margin="normal"
                    fullWidth
                    label="E-mail cím"
                    {...register('email', { required: 'Az e-mail megadása kötelező!' })}
                    error={!!errors.email}
                    helperText={errors.email?.message as string}
                />

                <TextField
                    margin="normal"
                    fullWidth
                    label="Jelszó"
                    type="password"
                    {...register('password', { required: 'A jelszó megadása kötelező!' })}
                    error={!!errors.password}
                    helperText={errors.password?.message as string}
                />

                <Button type="submit" fullWidth variant="contained" sx={{ mt: 3, mb: 2 }}>
                    Belépés
                </Button>
            </Box>
        </Paper>
    );
}