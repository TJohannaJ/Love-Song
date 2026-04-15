import { Button, Card, CardActions, CardContent, CardMedia, Typography } from "@mui/material";
import { ProfileCardProps } from "./profilecard";

export function LittleProfileCard({profile, handleLike, handleDislike}: {profile: ProfileCardProps, handleLike: (id: string) => void, handleDislike: (id: string) => void}) {
    const onCardLike = (e: React.MouseEvent<HTMLElement>) => {
        e.stopPropagation();
        handleLike(profile.id);
    };

    const onCardDislike = (e: React.MouseEvent<HTMLElement>) => {
        e.stopPropagation();
        handleDislike(profile.id);
    };
    return <Card sx={{ width: 310, height: 420 }}>
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
            <Button size="small" onClick={onCardLike}>
                Like
            </Button>
            <Button size="small" sx={{ justifyContent: "flex-end" }} onClick={onCardDislike}>
                Dislike
            </Button>
        </CardActions>
    </Card>;
}