import DeleteSweepIcon from '@mui/icons-material/DeleteSweep';
import { Button } from '@mui/material';
import { Profile } from '../../intefaces/iProfile';

interface ButtonProps {
    profile: Profile
    handleUndo?: (id: string) => void;
}
export function UndoButton({ profile, handleUndo }: ButtonProps) {
    return <Button 
            variant="outlined" 
            color="warning" 
            size="small"
            onClick={(e) => { e.stopPropagation(); handleUndo?.(profile.id); }}
          >
            Visszavonás
          </Button>
    }