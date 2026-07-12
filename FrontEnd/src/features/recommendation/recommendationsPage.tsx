import Box from '@mui/material/Box';
import Grid from '@mui/material/Grid'; 
import { CircularProgress, Typography } from '@mui/material';
import ClickableProfileCard from '../../components/common/clickableProfileCard';
import { useRecommendations } from '../../hooks/useRecommendations';

export function Recommendations() {
    const { profiles, loading, handleLike, handleDislike } = useRecommendations();

    const manageInteraction = (id: string, type: string) => {
        if (type === 'like') {
            handleLike(id);
        } else {
            handleDislike(id);
        }
    };

    if (loading) {
        return (
            <Box display="flex" justifyContent="center" py={5}>
                <CircularProgress />
            </Box>
        );
    }

    if (!profiles || profiles.length === 0) {
        return (
            <Box textAlign="center" py={5}>
                <Typography color="text.secondary">
                    Jelenleg nincsenek új ajánlataink számodra.
                </Typography>
            </Box>
        );
    }

    return (
        <Box sx={{ flexGrow: 1, p: 2 }}>
            <Grid container spacing={2} columns={{ xs: 2, sm: 4, md: 8, lg: 12 }}>
                {profiles.map(recommendation => (
                    <Grid key={recommendation.id} size={{ xs: 2, sm: 4, md: 4 }}>
                        <ClickableProfileCard 
                            profile={recommendation} 
                            onInteraction={(id) => manageInteraction(id, 'like')} 
                            viewType="little" 
                        />
                    </Grid>
                ))}
            </Grid>
        </Box>
    );
}