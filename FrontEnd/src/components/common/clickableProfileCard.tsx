import * as React from 'react';
import ButtonBase from '@mui/material/ButtonBase';
import Dialog from '@mui/material/Dialog';
import { DetailedProfile } from './detailedProfile';
import { Profile } from '../../intefaces/iProfile';
import { NotificationProfile } from '../../features/likes/notificationProfile';
import { LittleProfile } from '../../features/recommendation/littleProfile';

export type ProfileViewType = 'notification' | 'little' | 'likeHistory' | 'dislikeHistory';

export interface ProfileCardProps {
  profile: Profile;
  onInteraction: (id: string, type: 'like' | 'dislike' | 'undo') => void;
  viewType: ProfileViewType;
}

export default function ClickableProfileCard({ profile, onInteraction, viewType }: ProfileCardProps) {
  const [open, setOpen] = React.useState(false);

  const handleLike = () => {
    onInteraction(profile.id, 'like');
    setOpen(false);
  };

  const handleDislike = () => {
    onInteraction(profile.id, 'dislike');
    setOpen(false);
  };

  const handleUndo = () => {
    onInteraction(profile.id, 'undo');
    setOpen(false);
  };

  const renderSmallProfile = () => {
    if (viewType === 'little') {
      return <LittleProfile profile={profile} handleLike={handleLike} handleDislike={handleDislike} />;      
    }    
    return (
        <NotificationProfile 
          profile={profile} 
          handleLike={handleLike} 
          handleDislike={handleDislike}
          handleUndoDislike={handleUndo} 
          handleUndoLike={handleUndo}
          viewType={viewType}
        />
      );
  };

  return (
    <React.Fragment>
      <ButtonBase onClick={() => setOpen(true)} sx={{ width: '100%', overflow: 'visible' }}>
        {renderSmallProfile()}
      </ButtonBase>
      <Dialog open={open} onClose={() => setOpen(false)} scroll="paper">
        <DetailedProfile 
            profile={profile} 
            handleLike={handleLike} 
            handleDislike={handleDislike} 
            handleUndoDislike={handleUndo} 
            handleUndoLike={handleUndo}
            viewType={viewType} 
            open={open} 
        />
      </Dialog>
    </React.Fragment>
  );
}