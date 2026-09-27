import { Box, Button } from "@mui/material";
import { LikeTab } from "../../hooks/useLikes";

interface Props {
    activeTab: LikeTab;
    onTabChange: (tab: LikeTab) => void;
}

export const LikeTabs = ({ activeTab, onTabChange }: Props) => {
    const tabs: { id: LikeTab; label: string }[] = [
        { id: 'likes', label: 'Your Likes' },
        { id: 'likedYou', label: 'They liked you' },
        { id: 'dislikes', label: 'Your Dislikes' },
    ];

    return (
        <Box display="flex" justifyContent="center" gap={2} sx={{ my: 2, flexWrap: 'wrap' }}>
            {tabs.map((tab) => {
                const isActive = activeTab === tab.id;
                return (
                    <Button
                        key={tab.id}
                        size={isActive ? 'large' : 'medium'}
                        onClick={() => onTabChange(tab.id)}
                        sx={{
                            borderRadius: '20px',
                            textTransform: 'none',
                            fontWeight: 'bold',
                            transition: 'all 0.2s ease-in-out',
                            ...(isActive
                                ? {
                                    background: 'linear-gradient(135deg, #FF007A 0%, #FF5E00 100%)',
                                    color: '#ffffff',
                                    boxShadow: '0 4px 12px rgba(255, 0, 122, 0.35)',
                                    '&:hover': {
                                        background: 'linear-gradient(135deg, #E0006C 0%, #E05300 100%)',
                                        boxShadow: '0 6px 16px rgba(255, 0, 122, 0.45)',
                                    },
                                }
                                : {
                                    background: 'transparent',
                                    color: '#FF007A',
                                    border: '2px solid transparent',
                                    backgroundImage: 'linear-gradient(#fff, #fff), linear-gradient(135deg, #FF007A 0%, #FF5E00 100%)',
                                    backgroundOrigin: 'border-box',
                                    backgroundClip: 'padding-box, border-box',
                                    '&:hover': {
                                        backgroundColor: 'rgba(255, 0, 122, 0.05)',
                                    },
                                }),
                        }}
                    >
                        {tab.label}
                    </Button>
                );
            })}
        </Box>
    );
};