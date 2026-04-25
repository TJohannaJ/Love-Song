import { useEffect, useState } from "preact/hooks";
import { ChangeEvent, FormEvent } from 'preact/compat';
import { Box, TextField, Button, Typography, Paper, Container, CircularProgress, MenuItem, CardMedia } from "@mui/material";
import api from "../../api/axiosInstance";
import { User } from "../../intefaces/iUser";

export function ProfilePage() {
    const [profile, setProfile] = useState<User | null>(null);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        getProfileDatas();
    }, []);

    const handlePictureChange = (e: ChangeEvent<HTMLInputElement>) => {
        const file = (e.target as HTMLInputElement).files?.[0];
        if (file) {
            const reader = new FileReader();
            reader.onloadend = () => {
                const base64String = reader.result as string;
                const cleanBase64 = base64String.split(',')[1]; 

                setProfile(prev => prev ? { ...prev, picture: cleanBase64 } : null);
            };
            reader.readAsDataURL(file);
        }
    };

    async function getProfileDatas() {
        try {
            const response = await api.get('/User');
            setProfile(response.data);
        } catch (error) {
            console.error("Hiba történt a letöltéskor:", error);
        } finally {
            setLoading(false);
        }
    }

    // Univerzális input kezelő
    const handleChange = (e: ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) => {
        const target = e.target as HTMLInputElement | HTMLTextAreaElement;
        const { name, value } = target;
        setProfile(prev => prev ? { ...prev, [name]: value } : null);
    };

    const handleUpdate = async (e: FormEvent) => {
        e.preventDefault(); // Megállítjuk az oldal újratöltését
        if (!profile) return;

        try {
            // Itt a teljes profile objektumot elküldjük
            await api.patch('/User/updateProfile', profile);
            alert("Sikeres mentés!");
        } catch (err) {
            console.error("Hiba a mentésnél", err);
            alert("Hiba történt a mentés során.");
        }
    };

    if (loading) {
        return (
            <Box display="flex" justifyContent="center" alignItems="center" minHeight="50vh">
                <CircularProgress />
            </Box>
        );
    }

    return (
        <Container maxWidth="md">
            <Paper elevation={3} sx={{ p: 4, mt: 4 }}>
                <Typography variant="h5" gutterBottom>
                    Profil szerkesztése
                </Typography>
                
                <Box component="form" onSubmit={handleUpdate} noValidate sx={{ mt: 2 }}>                    
                    <Box display="flex" flexWrap="wrap" gap={3}>
                        <Box width="100%" display="flex" flexDirection="column" alignItems="center" gap={2} sx={{ border: '1px solid #ccc', padding: 2 }}>
                            <CardMedia component="img" alt="Profile Picture" image={profile?.picture ? `data:image/png;base64,${profile.picture}` : '/default-profile.png'}                                 
                                sx={{ width: 300, height: 300 }}
                            />
                            <Button
                                component="label"
                                variant="outlined"
                            >
                                Kép feltöltése
                                <input
                                    type="file"
                                    hidden
                                    accept="image/png"
                                    onChange={handlePictureChange}
                                />
                            </Button>
                        </Box>
                        <Box width={{ xs: '100%', sm: 'calc(50% - 12px)' }} display="flex" flexDirection="column" alignItems="center">
                            <TextField
                                fullWidth
                                label="Teljes név"
                                name="fullName"
                                value={profile?.fullName || ''}
                                onChange={handleChange}
                            />
                        </Box>

                        <Box width={{ xs: '100%', sm: 'calc(50% - 12px)' }} display="flex" flexDirection="column" alignItems="center">
                            <TextField
                                fullWidth
                                label="Város"
                                name="city"
                                value={profile?.city || ''}
                                onChange={handleChange}
                            />
                        </Box>

                        <Box width={{ xs: '100%', sm: 'calc(50% - 12px)' }} display="flex" flexDirection="column" alignItems="center">
                            <TextField
                                fullWidth
                                select
                                label="Nem"
                                name="gender"
                                value={profile?.gender || ''}
                                onChange={handleChange}
                            >
                                <MenuItem value="male">Férfi</MenuItem>
                                <MenuItem value="female">Nő</MenuItem>
                                <MenuItem value="other">Egyéb</MenuItem>
                            </TextField>
                        </Box>

                        <Box width="100%" display="flex" flexDirection="column" alignItems="center">
                            <TextField
                                fullWidth
                                label="Rövid bemutatkozás"
                                name="shortIntro"
                                value={profile?.shortIntro || ''}
                                onChange={handleChange}
                                variant="outlined"
                            />
                        </Box>

                        <Box width="100%" display="flex" flexDirection="column" alignItems="center">
                            <TextField
                                fullWidth
                                multiline
                                rows={4}
                                label="Részletes bemutatkozás"
                                name="longIntro"
                                value={profile?.longIntro || ''}
                                onChange={handleChange}
                            />
                        </Box>

                        <Box width="100%" display="flex" flexDirection="column" alignItems="center">
                            <Button 
                                type="submit" 
                                variant="contained" 
                                size="large" 
                                color="primary"
                                sx={{ px: 4 }}
                            >
                                Mentés
                            </Button>
                        </Box>
                    </Box>
                </Box>
            </Paper>
        </Container>
    );
}