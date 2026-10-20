import { Button, Card, CardActions, CardContent, CardMedia, Typography } from "@mui/material";
import { Profile } from "../../intefaces/iProfile";
import { LikeButton } from "../../components/common/likeButton";
import { DislikeButton } from "../../components/common/dislikeButton";

export function LittleProfile({profile, handleLike, handleDislike}: {profile: Profile, handleLike: (id: string) => void, handleDislike: (id: string) => void}) {
    return (
        <Card 
            sx={{ 
                width: '100%', 
                height: 420, 
                display: 'flex',
                flexDirection: 'column',
                justifyContent: 'space-between',
                textAlign: 'left', 
                transition: '0.3s ease', 
                borderRadius: 2, 
                '&:hover': { 
                    boxShadow: 6, 
                    backgroundColor: 'rgba(0, 0, 0, 0.02)'
                }
            }}
        >
            <CardMedia
                component="img"
                alt="Profile Picture"
                sx={{
                    height: 220,
                    width: '100%',
                    objectFit: 'cover',
                }}
                image={profile.pictureUrl || '/default-profile.png'} // Base64 prefix eltávolítva
            />
            <CardContent sx={{ flexGrow: 1 }}>
                <Typography gutterBottom variant="h5" component="div">
                    {profile.fullName}, {profile.age}
                </Typography>
                <Typography variant="body2" sx={{ color: 'text.secondary', textAlign: 'justify' }}>
                    {profile.shortIntro}
                </Typography>
            </CardContent>
            <CardActions sx={{ p: 2, pt: 0 }}>
                <LikeButton profile={profile} handleLike={handleLike} />
                <DislikeButton profile={profile} handleDislike={handleDislike} />
            </CardActions>
        </Card>
    );
}