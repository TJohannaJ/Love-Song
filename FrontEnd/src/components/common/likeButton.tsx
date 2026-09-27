import { IconButton } from "@mui/material";
import FavoriteIcon from '@mui/icons-material/Favorite';

export function LikeButton({ handleLike, profile }: { handleLike?: (id: string) => void, profile: { id: string } }) {
    return (
      <IconButton 
        onClick={(e) => {
          e.stopPropagation();
          handleLike?.(profile.id);
        }}
        sx={{ 
          background: 'linear-gradient(135deg, #FF007A 0%, #FF5E00 100%)',
          color: 'white',
          boxShadow: '0 2px 8px rgba(255, 0, 122, 0.3)',
          transition: 'transform 0.2s ease, box-shadow 0.2s ease',
          '&:hover': { 
            background: 'linear-gradient(135deg, #E0006C 0%, #E65300 100%)',
            transform: 'scale(1.08)',
            boxShadow: '0 4px 12px rgba(255, 0, 122, 0.4)',
          }
        }}
      >
        <FavoriteIcon />
      </IconButton>
    );
}