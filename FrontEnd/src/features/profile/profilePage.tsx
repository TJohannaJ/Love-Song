import {
    Box, TextField, Button, Typography, Paper, Container,
    CircularProgress, MenuItem, Snackbar, Alert
} from "@mui/material";
import { useProfile } from "../../hooks/useProfile";
import { ProfilePicture } from "./profilePicture";
import { SpotifyButton } from "../../components/common/spotifyButton";
import { useEffect, useState } from "react";
import { useSpotifyLink } from "../../hooks/useSpotify";
import { AiBioDialog } from "./AiBioDialog";

export function ProfilePage() {
    const { profile, loading, handleChange, handlePictureChange, updateProfile } = useProfile();
    const { handleLink, isRedirecting } = useSpotifyLink();
    const [openPopup, setOpenPopup] = useState(false);
    const [status, setStatus] = useState<{type: "success" | "error", msg: string} | null>(null);

    // AI Modal állapota
    const [aiDialogOpen, setAiDialogOpen] = useState(false);

    useEffect(() => {
        const handleMessage = (event: MessageEvent) => {
            if (event.data === 'spotify-success') {
                alert("Successfully linked Spotify account!");
            } else if (event.data === 'spotify-error') {
                alert("Failed to link Spotify account!");
            }
        };

        window.addEventListener('message', handleMessage);
        return () => window.removeEventListener('message', handleMessage);
    }, []);

    const handleClose = () => setOpenPopup(false);

    const handleAiSuccess = (shortIntro: string, longIntro: string) => {
        handleChange({
            target: { name: 'shortIntro', value: shortIntro },
            currentTarget: { name: 'shortIntro', value: shortIntro }
        } as any);

        handleChange({
            target: { name: 'longIntro', value: longIntro },
            currentTarget: { name: 'longIntro', value: longIntro }
        } as any);

        setStatus({ type: "success", msg: "AI bios generated successfully!" });
        setOpenPopup(true);
    };

    const handleAiError = (msg: string) => {
        setStatus({ type: "error", msg });
        setOpenPopup(true);
    };

    if (loading) return <CircularProgress />;

    return (
        <Container maxWidth="md">
            {/* Notification Popup */}
            <Snackbar open={openPopup} autoHideDuration={6000} onClose={handleClose} anchorOrigin={{ vertical: 'top', horizontal: 'center' }}>
                <Alert onClose={handleClose} severity={status?.type} sx={{ width: '100%' }}>
                    {status?.msg}
                </Alert>
            </Snackbar>

            {/* AI Dialog külön komponensként */}
            <AiBioDialog
                open={aiDialogOpen}
                onClose={() => setAiDialogOpen(false)}
                onSuccess={handleAiSuccess}
                onError={handleAiError}
            />

            {/* MAIN PROFILE EDIT FORM */}
            <Paper elevation={3} sx={{ p: 4, mt: 4 }}>
                <Typography variant="h5" gutterBottom>
                    Edit profile
                </Typography>

                <Box component="form" onSubmit={(e) => { e.preventDefault(); updateProfile(); }} sx={{ mt: 2 }}>
                    <Box display="flex" flexWrap="wrap" gap={3}>

                        <Box width="100%">
                            <SpotifyButton onClick={handleLink} loading={isRedirecting} />
                        </Box>
                        <ProfilePicture pictureUrl={profile?.pictureUrl} onPictureChange={handlePictureChange} />

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

                        {/* --- AI GOMB AZ INTRODUCTION MEZŐK FELETT --- */}
                        <Box width="100%" display="flex" justifyContent="center" mt={1}>
                            <Button
                                variant="contained"
                                onClick={() => setAiDialogOpen(true)}
                                sx={{
                                    px: 3,
                                    py: 1,
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
                                ✨ Generate Bio with AI
                            </Button>
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