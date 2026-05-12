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
        const spotifyStatus = searchParams.get("spotify");
        if (spotifyStatus === "success") {
            setStatus({ type: "success", msg: "Sikeres Spotify fiók összekapcsolás!" });
            setOpenPopup(true);
            setSearchParams({}, { replace: true });
        } else if (spotifyStatus === "error") {
            setStatus({ type: "error", msg: "Sikertelen összekapcsolás. Próbáld újra!" });
            setOpenPopup(true);
            setSearchParams({}, { replace: true });
        }
    }, [searchParams]);

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
                <Typography variant="h5" gutterBottom>Profil szerkesztése</Typography>

                <Box component="form" onSubmit={(e) => { e.preventDefault(); updateProfile(); }} sx={{ mt: 2 }}>
                    <Box display="flex" flexWrap="wrap" gap={3}>

                        <Box width="100%">
                            <SpotifyButton onClick={handleLink} loading={isRedirecting} />
                        </Box>
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