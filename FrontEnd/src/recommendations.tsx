import { styled } from '@mui/material/styles';
import Paper from '@mui/material/Paper';
import Box from '@mui/material/Box';
import Grid from '@mui/material/Grid';
import ProfileCard from './profilecard';
import { useEffect, useState } from 'preact/hooks';

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

export interface Profile{
    id: number;
    name: string;
    age: number;
    picture: string;
    short_intro : string;
    long_intro : string;
}

export function Recommendations() {
    const [profiles, setProfiles] = useState<Profile[]>();

    useEffect(() => {
        getProfileDatas();
    }, []);

    async function getProfileDatas() {
    const response = await fetch('user');
    if (response.ok) {
        const data = await response.json();
        setProfiles(data);
    }
}

    if (!profiles) {
        return <div>Loading...</div>;
    }

    return <Box sx={{ flexGrow: 1 }}>
        <Grid container spacing={{ xs: 2, md: 3 }} columns={{ xs: 4, sm: 8, md: 12 }}>
            {profiles.map(user => (
                <Grid key={user.id} size={{ xs: 2, sm: 4, md: 4 }}>
                    <ProfileCard {...user}></ProfileCard>
                </Grid>
            ))}
        </Grid>
    </Box>
}

