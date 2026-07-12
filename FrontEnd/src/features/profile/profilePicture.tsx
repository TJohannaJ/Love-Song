import { Box, CardMedia, Button } from "@mui/material";

interface Props {
    picture: string | undefined;
    onPictureChange: (e: any) => void;
}

export const ProfilePicture = ({ picture, onPictureChange }: Props) => (
    <Box width="100%" display="flex" flexDirection="column" alignItems="center" gap={2} sx={{ border: '1px solid #ccc', padding: 2 }}>
        <CardMedia 
            component="img" 
            alt="Profile Picture" 
            image={picture ? `data:image/png;base64,${picture}` : '/default-profile.png'} 
            sx={{ width: 300, height: 300, objectFit: 'cover' }}
        />
        <Button component="label" variant="outlined">
            Kép feltöltése
            <input type="file" hidden accept="image/png" onChange={onPictureChange} />
        </Button>
    </Box>
);