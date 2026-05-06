import { Box, CircularProgress, Divider, List, Paper, Typography } from "@mui/material";
import ClickableProfileCard from "../../components/common/clickableProfileCard";
import { useLikes } from "../../hooks/useLikes";
import { LikeTabs } from "./likeTabs";

export function LikesPage() {
    const { 
        profiles, 
        loading, 
        activeTab, 
        setActiveTab, 
        handleAcceptLike, 
        handleDeclineLike, 
        handleUndo 
    } = useLikes();

    const emptyMessages = {
        likes: "Még nem kedveltél senkit.",
        dislikes: "Még nem dislikeoltál senkit.",
        likedYou: "Még nem kedvelt téged senki. Ne csüggedj!"
    };

    const viewTypeMap = {
        likes: 'likeHistory',
        dislikes: 'dislikeHistory',
        likedYou: 'notification'
    };

    // Eldönti, hogy az adott profilnál melyik funkciót hívjuk meg
    // A ClickableProfileCard onInteraction prop-jához igazítva
    const manageInteraction = (id: string, type: string) => {
        if (activeTab === 'likedYou') {
            type === 'like' ? handleAcceptLike(id) : handleDeclineLike(id);
        } else {
            // 'likes' vagy 'dislikes' fülön a gomb visszavonást (undo) jelent
            handleUndo(id);
        }
    };

    if (loading) return (
        <Box display="flex" justifyContent="center" alignItems="center" minHeight="50vh">
            <CircularProgress />
        </Box>
    );

    return (
        <Box>
            <LikeTabs activeTab={activeTab} onTabChange={setActiveTab} />

            <Paper elevation={2}>
                <List sx={{ width: '100%', bgcolor: 'background.paper', borderRadius: 2 }}>
                    {profiles && profiles.length > 0 ? (
                        profiles.map((profile, index) => (
                            <Box key={profile.id}>
                                <ClickableProfileCard 
                                    profile={profile} 
                                    onInteraction={(id, type) => manageInteraction(id, type)}
                                    viewType={viewTypeMap[activeTab] as any} 
                                />
                                {index < profiles.length - 1 && <Divider variant="fullWidth" component="li" />}
                            </Box>
                        ))
                    ) : (
                        <Box sx={{ p: 4, textAlign: 'center' }}>
                            <Typography variant="body1" color="text.secondary">
                                {emptyMessages[activeTab]}
                            </Typography>
                        </Box>
                    )}
                </List>
            </Paper>
        </Box>
    );
}