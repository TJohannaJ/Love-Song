import { useEffect, useState } from 'react';
import { styled } from '@mui/material/styles';
import Paper from '@mui/material/Paper';
import Box from '@mui/material/Box';
import Grid from '@mui/material/Grid';
import ProfileCard from './profilecard';

const Item = styled(Paper)(({ theme }) => ({
    backgroundColor: '#fff',
    ...theme.typography.body2,
    padding: theme.spacing(2),
    textAlign: 'center',
    color: (theme.vars ?? theme).palette.text.secondary,
    ...theme.applyStyles('dark', {
        backgroundColor: '#1A2027',
    }),
}));

export interface Profile {
    id: number;
    fullName: string;
    age: number;
    gender: string;
    city: string;
    preferences: string;
    picture: string;
    shortIntro: string;
    longIntro: string;
}

export function Recommendations() {
    const [profiles, setProfiles] = useState<Profile[]>();    

    useEffect(() => {
        getProfileDatas();
    }, []);

    async function getProfileDatas() {
        const response = await fetch('https://localhost:7160/Recommendation/4');
        if (response.ok) {
            const data = await response.json();
            setProfiles(data);
        }
    }

    if (!profiles) {
        return <div>Loading...</div>;
    }

    return <Box sx={{ flexGrow: 1 }}>
        <Grid container alignContent="center" spacing={{ xs: 1, md: 1 }} columns={{ xs: 2, sm: 4, md: 8, lg: 12 }}>
            {profiles.map(recommendation => (
                <Grid alignContent="center" key={recommendation.id} size={{ xs: 2, sm: 4, md: 4 }}>
                    <ProfileCard {...recommendation}></ProfileCard>
                </Grid>
            ))}            
        </Grid>
    </Box>
    
}

