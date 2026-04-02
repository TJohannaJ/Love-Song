import { useForm } from 'react-hook-form';
import { Box, Button, TextField, Typography, Paper, MenuItem, Link } from '@mui/material';
import { Link as RouterLink } from 'react-router-dom';

export function RegisterForm() {
    const { register, handleSubmit, watch, formState: { errors } } = useForm();

    const onSubmit = async (data: any) => {
        const { confirmPassword, ...registerData } = data;
        try {
            const response = await fetch('https://localhost:7160/api/Account/register', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(registerData),
            });

            const result = await response.json();

            if (response.ok) {
                alert('Sikeres regisztráció! Most már bejelentkezhetsz.');
            } else {
                alert(result.message || 'Hiba történt a regisztráció során!');
            }
        } catch (error) {
            console.error('Network error:', error);
        }
    };

    return (
        <Paper sx={{ p: 4, maxWidth: 500, mx: 'auto', mt: 5 }}>
            <Typography variant="h5" mb={2}>Regisztráció</Typography>

            <Box sx={{ textAlign: 'center' }}>
                <Typography variant="body2">
                    Már van fiókod?{' '}
                    <Link component={RouterLink} to="/login" sx={{ cursor: 'pointer', fontWeight: 'bold' }}>
                        Jelentkezz be!
                    </Link>
                </Typography>
            </Box>
            <Box component="form" onSubmit={handleSubmit(onSubmit)} noValidate>
                <TextField
                    margin="normal"
                    fullWidth
                    label="Teljes név"
                    {...register('fullName', { required: 'A név megadása kötelező!' })}
                    error={!!errors.fullName}
                    helperText={errors.fullName?.message as string}
                />

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
                    label="Születési dátum"
                    type="date"
                    slotProps={{ inputLabel: { shrink: true } }}
                    {...register('birthDate', { required: 'A születési dátum megadása kötelező!' })}
                    error={!!errors.birthDate}
                    helperText={errors.birthDate?.message as string}
                />

                <TextField
                    margin="normal"
                    fullWidth
                    label="Tartózkodási város"
                    {...register('city', { required: 'A város megadása kötelező!' })}
                    error={!!errors.city}
                    helperText={errors.city?.message as string}
                />

                <TextField
                    select
                    margin="normal"
                    fullWidth
                    label="Nem"
                    defaultValue=""
                    {...register('gender', { required: 'A nemed kiválasztása kötelező!' })}
                    error={!!errors.gender}
                    helperText={errors.gender?.message as string}
                >
                    <MenuItem value="no">Nő</MenuItem>
                    <MenuItem value="ferfi">Férfi</MenuItem>
                    <MenuItem value="egyeb">Egyéb</MenuItem>
                </TextField>

                <TextField
                    select
                    margin="normal"
                    fullWidth
                    label="Kit keresel? (Preferencia)"
                    defaultValue=""
                    {...register('preferences', { required: 'A preferencia kiválasztása kötelező!' })}
                    error={!!errors.preferences}
                    helperText={errors.preferences?.message as string}
                >
                    <MenuItem value="no">Nőket</MenuItem>
                    <MenuItem value="ferfi">Férfiakat</MenuItem>
                    <MenuItem value="mindketto">Mindkettőt</MenuItem>
                </TextField>

                <TextField
                    margin="normal"
                    fullWidth
                    label="Jelszó"
                    type="password"
                    {...register('password', { required: 'A jelszó megadása kötelező!' })}
                    error={!!errors.password}
                    helperText={errors.password?.message as string}
                />

                <TextField
                    margin="normal"
                    fullWidth
                    label="Jelszó újra"
                    type="password"
                    {...register('confirmPassword', {
                        required: 'A jelszó megerősítése kötelező!',
                        validate: (val: string) => watch('password') === val || "A jelszavak nem egyeznek!"
                    })}
                    error={!!errors.confirmPassword}
                    helperText={errors.confirmPassword?.message as string}
                />

                <Button type="submit" fullWidth variant="contained" color="success" sx={{ mt: 3, mb: 2 }}>
                    Fiók létrehozása
                </Button>
            </Box>
        </Paper>
    );
}