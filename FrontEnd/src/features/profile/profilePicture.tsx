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
        <Button 
            component="label" 
            sx={{
                borderRadius: '20px',
                px: 3,
                py: 1,
                fontWeight: 'bold',
                color: '#FF007A',
                border: '2px solid transparent',
                backgroundImage: 'linear-gradient(#fff, #fff), linear-gradient(135deg, #FF007A 0%, #FF5E00 100%)',
                backgroundOrigin: 'border-box',
                backgroundClip: 'padding-box, border-box',
                transition: 'all 0.2s ease-in-out',
                '&:hover': {
                    background: 'linear-gradient(135deg, #FF007A 0%, #FF5E00 100%)',
                    color: '#ffffff',
                    boxShadow: '0 4px 12px rgba(255, 0, 122, 0.35)',
                },
            }}
        >
            Kép feltöltése
            <input type="file" hidden accept="image/png" onChange={onPictureChange} />
        </Button>
    </Box>
);