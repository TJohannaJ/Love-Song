import { IconButton } from "@mui/material";
import FavoriteIcon from '@mui/icons-material/Favorite';

export function LikeButton({ handleLike, profile }: { handleLike?: (id: string) => void, profile: { id: string } }) {
    return <IconButton 
          onClick={(e) => {
            e.stopPropagation();
            handleLike?.(profile.id);
          }}
          sx={{ 
            color: '#b6174c', 
            border: '1px solid', 
            borderColor: '#d94f7d',
            '&:hover': { 
              backgroundColor: '#b6174c', 
              color: 'white',
              borderColor: '#b6174c'
            }
          }}
        >
          <FavoriteIcon />
        </IconButton>
}