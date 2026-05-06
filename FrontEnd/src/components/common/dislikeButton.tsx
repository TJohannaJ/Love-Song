import { IconButton } from "@mui/material";
import CloseIcon from '@mui/icons-material/Close';

export function DislikeButton({ handleDislike, profile }: { handleDislike?: (id: string) => void, profile: { id: string } }) {
    return <IconButton 
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
}