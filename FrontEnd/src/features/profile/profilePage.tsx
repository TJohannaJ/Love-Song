import { Box, TextField, Button, Typography, Paper, Container, CircularProgress, MenuItem, Snackbar, Alert } from "@mui/material";
import { useProfile } from "../../hooks/useProfile";
import { ProfilePicture } from "./profilePicture";
import { SpotifyButton } from "../../components/common/spotifyButton";
import { useSearchParams } from "react-router-dom";
import { useEffect, useState } from "react";
import { useSpotifyLink } from "../../hooks/useSpotify";

export function ProfilePage() {
    const { profile, loading, handleChange, handlePictureChange, updateProfile } = useProfile();
    const { handleLink, isRedirecting } = useSpotifyLink();
    const [searchParams, setSearchParams] = useSearchParams();
    const [openPopup, setOpenPopup] = useState(false);
    const [status, setStatus] = useState<{type: "success" | "error", msg: string} | null>(null);

    useEffect(() => {
        const handleMessage = (event: MessageEvent) => {
            if (event.data === 'spotify-success') {
                alert("Sikeres Spotify fiók összekapcsolás!");
            } else if (event.data === 'spotify-error') {
                alert("Sikertelen összekapcsolás!");
            }
        };

        window.addEventListener('message', handleMessage);
        return () => window.removeEventListener('message', handleMessage);
    }, []);

    const handleClose = () => setOpenPopup(false);

    if (loading) return <CircularProgress />;

    return (
        <Container maxWidth="md">
            {/* Popup ablak (Snackbar) */}
            <Snackbar open={openPopup} autoHideDuration={6000} onClose={handleClose} anchorOrigin={{ vertical: 'top', horizontal: 'center' }}>
                <Alert onClose={handleClose} severity={status?.type} sx={{ width: '100%' }}>
                    {status?.msg}
                </Alert>
            </Snackbar>

            <Paper elevation={3} sx={{ p: 4, mt: 4 }}>
                <Typography variant="h5" gutterBottom>Edit profile</Typography>

                <Box component="form" onSubmit={(e) => { e.preventDefault(); updateProfile(); }} sx={{ mt: 2 }}>
                    <Box display="flex" flexWrap="wrap" gap={3}>

                        <Box width="100%">
                            <SpotifyButton onClick={handleLink} loading={isRedirecting} />
                        </Box>
                        <ProfilePicture picture={profile?.picture} onPictureChange={handlePictureChange} />

                        <Box width={{ xs: '100%', sm: 'calc(50% - 12px)' }}>
                            <TextField fullWidth label="Full Name" name="fullName" value={profile?.fullName || ''} onChange={handleChange} />
                        </Box>

                        <Box width={{ xs: '100%', sm: 'calc(50% - 12px)' }}>
                            <TextField fullWidth label="City" name="city" value={profile?.city || ''} onChange={handleChange} />
                        </Box>

                        <Box width={{ xs: '100%', sm: 'calc(50% - 12px)' }}>
                            <TextField fullWidth select label="Gender" name="gender" value={profile?.gender || ''} onChange={handleChange}>
                                <MenuItem value="male">Male</MenuItem>
                                <MenuItem value="female">Female</MenuItem>
                                <MenuItem value="other">Other</MenuItem>
                            </TextField>
                        </Box>

                        <TextField fullWidth label="Short Introduction" name="shortIntro" value={profile?.shortIntro || ''} onChange={handleChange} />

                        <TextField fullWidth multiline rows={4} label="Detailed Introduction" name="longIntro" value={profile?.longIntro || ''} onChange={handleChange} />

                        <Box width="100%" display="flex" justifyContent="center">
                            <Button 
                                type="submit" 
                                size="large" 
                                sx={{ 
                                    px: 5,
                                    py: 1.2,
                                    borderRadius: '25px',
                                    fontWeight: 'bold',
                                    color: '#ffffff',
                                    background: 'linear-gradient(135deg, #FF007A 0%, #FF5E00 100%)',
                                    boxShadow: '0 4px 14px rgba(255, 0, 122, 0.4)',
                                    transition: 'all 0.2s ease-in-out',
                                    '&:hover': {
                                        background: 'linear-gradient(135deg, #E0006C 0%, #E05300 100%)',
                                        boxShadow: '0 6px 18px rgba(255, 0, 122, 0.55)',
                                        transform: 'translateY(-1px)',
                                    }
                                }}
                            >
                                Save
                            </Button>
                        </Box>
                    </Box>
                </Box>
            </Paper>
        </Container>
    );
}