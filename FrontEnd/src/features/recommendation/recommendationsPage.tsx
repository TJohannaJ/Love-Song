import { useEffect, useState } from 'react';
import Box from '@mui/material/Box';
import Grid from '@mui/material/Grid';
import api from '../../api/axiosInstance';
import { Profile } from '../../intefaces/iProfile';
import ClickableProfileCard from '../../components/common/clickableProfileCard';

export function Recommendations() {
    const [profiles, setProfiles] = useState<Profile[]>();

    useEffect(() => {
        getProfileDatas();
    }, []);

    async function getProfileDatas() {
        try {
            const response = await api.get('/Recommendation');
            setProfiles(response.data);
        } catch (error) {
            console.error("Hiba történt a letöltéskor:", error);
        }
    }

    const removeProfileFromList = (id: string) => {
        setProfiles((prev) => prev?.filter(p => p.id !== id) ?? []);
    };

    if (!profiles) {
        return <div>Loading...</div>;
    }

    return <Box sx={{ flexGrow: 1 }}>
        <Grid container alignContent="center" spacing={{ xs: 1, md: 1 }} columns={{ xs: 2, sm: 4, md: 8, lg: 12 }}>
            {profiles.map(recommendation => (
                <Grid alignContent="center" key={recommendation.id} size={{ xs: 2, sm: 4, md: 4 }}>
                    <ClickableProfileCard profile={recommendation} onInteraction={removeProfileFromList} viewType="little"></ClickableProfileCard>
                </Grid>
            ))}
        </Grid>
    </Box>
}
