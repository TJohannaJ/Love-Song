import Card from '@mui/material/Card';
import CardActions from '@mui/material/CardActions';
import CardContent from '@mui/material/CardContent';
import CardMedia from '@mui/material/CardMedia';
import Button from '@mui/material/Button';
import Typography from '@mui/material/Typography';
import ButtonBase from '@mui/material/ButtonBase';
import { Profile } from './recommendations';

export default function ProfileCard(profile: Profile) {
  return (
    <ButtonBase> 
    <Card sx={{ width: 280, height: 400 }}>
      <CardMedia
        component="img"
        alt="Profile Picture"
        height="200"
        image={`data:image\\png;base64,${profile.picture}`}
      />
      <CardContent>
        <Typography gutterBottom variant="h5" component="div">
          {profile.name}, {profile.age}
        </Typography>
        <Typography variant="body2" sx={{ color: 'text.secondary', textAlign: 'justify' }}>
          {profile.shortIntro}
        </Typography>
      </CardContent>
      <CardActions>
        <Button size="small">Like</Button>
        <Button size="small" sx={{ justifyContent: "flex-end" }}>Dislike</Button>
      </CardActions>
    </Card>
    </ButtonBase>
  );
}
