import { Button, Card, CardActions, CardContent, CardMedia, Typography } from "@mui/material";
import { Profile } from "../../intefaces/iProfile";
import { LikeButton } from "../likeButton";
import { DislikeButton } from "../dislikeButton";

export function LittleProfile({profile, handleLike, handleDislike}: {profile: Profile, handleLike: (id: string) => void, handleDislike: (id: string) => void}) {
    return <Card sx={{ width: 310, height: 420 , textAlign: 'left' , transition: '0.3s ease', borderRadius: 1, '&:hover': { boxShadow: 6, backgroundColor: 'rgba(0, 0, 0, 0.02)'}}}>
        <CardMedia component="img" alt="Profile Picture" height="200" image={`data:image/png;base64,${profile.picture}`} />
        <CardContent>
            <Typography gutterBottom variant="h5" component="div">
                {profile.fullName}, {profile.age}
            </Typography>
            <Typography variant="body2" sx={{ color: 'text.secondary', textAlign: 'justify' }}>
                {profile.shortIntro}
            </Typography>
        </CardContent>
        <CardActions>
            <LikeButton profile={profile} handleLike={handleLike} />
            <DislikeButton profile={profile} handleDislike={handleDislike} />
        </CardActions>
    </Card>;
}