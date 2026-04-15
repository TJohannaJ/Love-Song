import { useForm } from 'react-hook-form';
import { Box, Button, TextField, Typography, Paper, Link } from '@mui/material';
import { Link as RouterLink, useNavigate } from 'react-router-dom';
import { useAuth } from '../context/authContext';
import { useEffect, useState } from 'react';
import api from '../api/axiosInstance';

export function LoginForm() {
    const { register, handleSubmit, formState: { errors } } = useForm();
    const navigate = useNavigate();
    const { login, setOpenDrawer, isLoggedIn } = useAuth();

    const onSubmit = async (data: any) => {
        try {
            const response = await api.post('/api/Account/login', data);
            const result = response.data;

            login(result.token, result.refreshToken || "");
            setOpenDrawer(true);
            navigate('/', { replace: true });
        } catch (error: any) {
            alert(error.response?.data?.message || 'Hiba történt!');
        }
    };

    return (
        <Paper sx={{ p: 4, maxWidth: 400, mx: 'auto', mt: 5 }}>
            <Typography variant="h5" mb={2}>Bejelentkezés</Typography>

            <Box sx={{ textAlign: 'center' }}>
                <Typography variant="body2">
                    Nincs még fiókod?{' '}
                    <Link component={RouterLink} to="/register" sx={{ cursor: 'pointer', fontWeight: 'bold' }}>
                        Regisztrálj itt!
                    </Link>
                </Typography>
            </Box>

            <Box component="form" onSubmit={handleSubmit(onSubmit)} noValidate>
                <TextField
                    margin="normal"
                    fullWidth
                    label="E-mail cím"
                    type="email"
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