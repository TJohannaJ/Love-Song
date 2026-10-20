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
        likes: "You haven't liked anyone yet.",
        dislikes: "You haven't disliked anyone yet.",
        likedYou: "No one has liked you yet. Don't give up!"
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

            <Paper elevation={2}
                   sx={{
                       borderRadius: 3,
                       overflow: 'hidden'
                   }}>
                <List
                    sx={{
                        width: '100%',
                        bgcolor: 'background.paper',
                        borderRadius: 3,
                        overflow: 'visible',
                        p: 0,
                        '& > div:first-of-type .MuiListItem-root': {
                            borderTopLeftRadius: 12,
                            borderTopRightRadius: 12,
                        },
                        '& > div:last-of-type .MuiListItem-root': {
                            borderBottomLeftRadius: 12,
                            borderBottomRightRadius: 12,
                        },
                    }}
                >
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