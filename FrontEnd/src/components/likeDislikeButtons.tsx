import { Box, IconButton } from "@mui/material";
import FavoriteIcon from '@mui/icons-material/Favorite';
import CloseIcon from '@mui/icons-material/Close';

export function LikeDislikeButtons({ handleLike, handleDislike, profile }: { handleLike?: (id: string) => void, handleDislike?: (id: string) => void, profile: { id: string } }) {
    return <Box sx={{ display: 'flex', gap: 1, alignSelf: 'center', ml: 2 }}>
        <IconButton 
          onClick={(e) => {
            e.stopPropagation();
            handleDislike?.(profile.id);
          }}
          sx={{ 
            color: 'text.secondary', 
            border: '1px solid', 
            borderColor: 'divider', 
            '&:hover': { 
              backgroundColor: 'text.secondary', 
              color: 'white' 
            }
          }}
        >
          <CloseIcon />
        </IconButton>

        <IconButton 
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
      </Box>
}