import * as React from 'react';
import Box from '@mui/material/Box';
import Grid from '@mui/material/Grid';
import { CircularProgress, Typography } from '@mui/material';
import ClickableProfileCard from '../../components/common/clickableProfileCard';
import { useRecommendations } from '../../hooks/useRecommendations';
import { PreferenceFilters, FilterState } from './preferenceFilter';
import { socialService } from '../../services/socialService';
import { UserPreference } from '../../intefaces/iUserPreference';

export function Recommendations() {
    const { profiles, loading, handleLike, handleDislike, refresh } = useRecommendations();
    const [initialFilters, setInitialFilters] = React.useState<FilterState | undefined>(undefined);

    // 1. Meg meglévő preferenciák betöltése az oldal nyitásakor
    React.useEffect(() => {
        const loadPreferences = async () => {
            try {
                const res = await socialService.getPreferences();
                if (res.data) {
                    setInitialFilters({
                        ageRange: [res.data.minAge, res.data.maxAge],
                        gender: res.data.gender,
                        city: res.data.city || '',
                        commonSongOnly: res.data.commonSongOnly
                    });
                }
            } catch (error) {
                console.error("Hiba a preferenciák betöltésekor:", error);
            }
        };
        loadPreferences();
    }, []);

    const manageInteraction = (id: string, type: string) => {
        if (type === 'like') {
            handleLike(id);
        } else {
            handleDislike(id);
        }
    };

    // 2. Szűrők alkalmazása és frissített ajánlások lekérése
    const handleApplyFilters = async (filters: FilterState) => {
        try {
            const prefDto: UserPreference = {
                minAge: filters.ageRange[0],
                maxAge: filters.ageRange[1],
                gender: filters.gender,
                city: filters.city,
                commonSongOnly: filters.commonSongOnly
            };

            // Mentjük az új beállításokat az API-n keresztül
            await socialService.updatePreferences(prefDto);

            // Frissítjük a helyi inicializáló állapotot is!
            setInitialFilters(filters);

            // Újra lekérjük az ajánlásokat a frissített szűrők alapján
            await refresh();
        } catch (error) {
            console.error("Hiba a preferenciák mentésekor:", error);
        }
    };
    if (loading) {
        return (
            <Box display="flex" justifyContent="center" py={5}>
                <CircularProgress sx={{ color: '#FF007A' }} />
            </Box>
        );
    }

    return (
        <Box sx={{ flexGrow: 1, p: 2 }}>
            {/* Szűrő komponens átadott kezdeti adatokkal és unique key-vel */}
            <PreferenceFilters
                key={initialFilters ? JSON.stringify(initialFilters) : 'default-filters'}
                initialPreferences={initialFilters}
                onApplyFilters={handleApplyFilters}
            />

            {/* Ajánlott profilok listája */}
            {!profiles || profiles.length === 0 ? (
                <Box textAlign="center" py={5}>
                    <Typography color="text.secondary">
                        Currently, there are no recommendations available. Please try again later.
                    </Typography>
                </Box>
            ) : (
                <Grid container spacing={2} columns={{ xs: 2, sm: 4, md: 8, lg: 12 }}>
                    {profiles.map((recommendation) => (
                        <Grid key={recommendation.id} size={{ xs: 2, sm: 4, md: 4 }}>
                            <ClickableProfileCard
                                profile={recommendation}
                                onInteraction={(id, type) => manageInteraction(id, type)}
                                viewType="little"
                            />
                        </Grid>
                    ))}
                </Grid>
            )}
        </Box>
    );
}