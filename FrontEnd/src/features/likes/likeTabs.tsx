import { Box, Button } from "@mui/material";
import { LikeTab } from "../../hooks/useLikes";

interface Props {
    activeTab: LikeTab;
    onTabChange: (tab: LikeTab) => void;
}

export const LikeTabs = ({ activeTab, onTabChange }: Props) => {
    const tabs: { id: LikeTab; label: string }[] = [
        { id: 'likes', label: 'Likejaid' },
        { id: 'likedYou', label: 'Tetszettél nekik' },
        { id: 'dislikes', label: 'Dislikejaid' },
    ];

    return (
        <Box display="flex" justifyContent="center" gap={2} sx={{ my: 2 }}>
            {tabs.map((tab) => (
                <Button
                    key={tab.id}
                    size={activeTab === tab.id ? 'large' : 'medium'}
                    variant={activeTab === tab.id ? 'contained' : 'outlined'}
                    onClick={() => onTabChange(tab.id)}
                >
                    {tab.label}
                </Button>
            ))}
        </Box>
    );
};