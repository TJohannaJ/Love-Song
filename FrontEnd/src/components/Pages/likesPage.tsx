import { Avatar, Box, Button, CircularProgress, Divider, List, ListItem, ListItemAvatar, ListItemText, Paper, Typography } from "@mui/material";
import { useEffect, useState } from "preact/hooks";
import api from "../../api/axiosInstance";
import React from "preact/compat";
import { NotificationProfile } from "../ProfileCards/notificationProfile";
import { Profile } from "../../intefaces/iProfile";
import ClickableProfileCard from "../ProfileCards/clickableProfileCard";

export function LikesPage() {
    const [profiles, setProfiles] = useState<Profile[] | null>(null);
    const [loading, setLoading] = useState(true);
    const [activeTab, setActiveTab] = useState<'likes' | 'likedYou' | 'dislikes'>('likedYou');

    useEffect(() => {
        let isCurrent = true;

        async function fetchProfiles() {
            setLoading(true);
            try {
                const response = await api.get(`/Likes/${activeTab}`);
                if (isCurrent) {
                    setProfiles(response.data);
                }
            } catch (error) {
                console.error("Hiba történt a letöltéskor:", error);
            } finally {
                if (isCurrent) setLoading(false);
            }
        }

        fetchProfiles();
        return () => { isCurrent = false; };
    }, [activeTab]);

    const getEmptyMessage = () => {
        switch (activeTab) {
            case 'likes': return "Még nem kedveltél senkit.";
            case 'dislikes': return "Még nem dislikeoltál senkit.";
            default: return "Még nem kedvelt téged senki. Ne csüggedj!";
        }
    };

    const removeProfileFromList = (id: string) => {
        setProfiles((prev) => prev?.filter(p => p.id !== id) ?? []);
    };

    if (loading) {
        return (
            <Box display="flex" justifyContent="center" alignItems="center" minHeight="50vh">
                <CircularProgress />
            </Box>
        );
    }
    return <div>
        <Box display="flex" justifyContent="center" gap={2} sx={{ my: 2 }}>
            <Button 
                size={activeTab === 'likes' ? 'large' : 'medium'} 
                variant={activeTab === 'likes' ? 'contained' : 'outlined'}
                onClick={() => setActiveTab('likes')}
            > Likejaid
            </Button>
            <Button 
                size={activeTab === 'likedYou' ? 'large' : 'medium'} 
                variant={activeTab === 'likedYou' ? 'contained' : 'outlined'}
                onClick={() => setActiveTab('likedYou')}
            > Tetszettél nekik
            </Button>
            <Button 
                size={activeTab === 'dislikes' ? 'large' : 'medium'} 
                variant={activeTab === 'dislikes' ? 'contained' : 'outlined'}
                onClick={() => setActiveTab('dislikes')}
            > Dislikejaid
            </Button>
        </Box>
        <Paper elevation={2}>
            <List sx={{ width: '100%', bgcolor: 'background.paper', borderRadius: 2 }}>
                {profiles && profiles.length > 0 ? (
                    profiles.map((profile, index) => (
                        <Box key={profile.id} sx={{ width: '100%', textAlign: 'center' }}>
                            <ClickableProfileCard profile={profile} onInteraction={removeProfileFromList} viewType="notification"></ClickableProfileCard>
                            {/* Csak akkor teszünk elválasztót, ha nem ez az utolsó elem */}
                            {index < profiles.length - 1 && (
                                <Divider variant="fullWidth" component="li" />
                            )}
                        </Box>
                    ))
                ) : (
                    <Box sx={{ p: 4, textAlign: 'center' }}>
                        <Typography variant="body1" color="text.secondary">
                            {getEmptyMessage()}
                        </Typography>
                    </Box>
                )}
            </List>
        </Paper>

    </div>;
}