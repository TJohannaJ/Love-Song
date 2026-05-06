import { Box, TextField, Button, Typography, Paper, Container, CircularProgress, MenuItem } from "@mui/material";
import { useProfile } from "../../hooks/useProfile";
import { ProfilePicture } from "./profilePicture";

export function ProfilePage() {
    const { profile, loading, handleChange, handlePictureChange, updateProfile } = useProfile();

    if (loading) return (
        <Box display="flex" justifyContent="center" alignItems="center" minHeight="50vh">
            <CircularProgress />
        </Box>
    );

    return (
        <Container maxWidth="md">
            <Paper elevation={3} sx={{ p: 4, mt: 4 }}>
                <Typography variant="h5" gutterBottom>Profil szerkesztése</Typography>
                
                <Box component="form" onSubmit={(e) => { e.preventDefault(); updateProfile(); }} sx={{ mt: 2 }}>
                    <Box display="flex" flexWrap="wrap" gap={3}>
                        
                        <ProfilePicture picture={profile?.picture} onPictureChange={handlePictureChange} />

                        <Box width={{ xs: '100%', sm: 'calc(50% - 12px)' }}>
                            <TextField fullWidth label="Teljes név" name="fullName" value={profile?.fullName || ''} onChange={handleChange} />
                        </Box>

                        <Box width={{ xs: '100%', sm: 'calc(50% - 12px)' }}>
                            <TextField fullWidth label="Város" name="city" value={profile?.city || ''} onChange={handleChange} />
                        </Box>

                        <Box width={{ xs: '100%', sm: 'calc(50% - 12px)' }}>
                            <TextField fullWidth select label="Nem" name="gender" value={profile?.gender || ''} onChange={handleChange}>
                                <MenuItem value="male">Férfi</MenuItem>
                                <MenuItem value="female">Nő</MenuItem>
                                <MenuItem value="other">Egyéb</MenuItem>
                            </TextField>
                        </Box>

                        <TextField fullWidth label="Rövid bemutatkozás" name="shortIntro" value={profile?.shortIntro || ''} onChange={handleChange} />
                        
                        <TextField fullWidth multiline rows={4} label="Részletes bemutatkozás" name="longIntro" value={profile?.longIntro || ''} onChange={handleChange} />

                        <Box width="100%" display="flex" justifyContent="center">
                            <Button type="submit" variant="contained" size="large" sx={{ px: 4 }}>Mentés</Button>
                        </Box>
                    </Box>
                </Box>
            </Paper>
        </Container>
    );
}