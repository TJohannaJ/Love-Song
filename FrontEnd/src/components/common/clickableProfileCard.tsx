import * as React from 'react';
import ButtonBase from '@mui/material/ButtonBase';
import Dialog from '@mui/material/Dialog';
import api from '../../api/axiosInstance';
import { DetailedProfile } from './detailedProfile';
import { Profile } from '../../intefaces/iProfile';
import { NotificationProfile } from '../../features/recommendation/notificationProfile';
import { LittleProfile } from '../../features/recommendation/littleProfile';

export type ProfileViewType = 'notification' | 'little' | 'likeHistory' | 'dislikeHistory';

export interface ProfileCardProps {
  profile: Profile;
  onInteraction: (id: string) => void;
  viewType: ProfileViewType;
}

export default function ClickableProfileCard({ profile, onInteraction, viewType }: ProfileCardProps) {
  const [open, setOpen] = React.useState(false);
  
 const renderSmallProfile = () => {
    if (viewType === 'little') {
      return <LittleProfile profile={profile} handleLike={handleLike} handleDislike={handleDislike} />;      
    }    
    return (
        <NotificationProfile 
          profile={profile} 
          handleLike={handleLike} 
          handleDislike={handleDislike}
          handleUndoDislike={handleUndoDislike}
          handleUndoLike={handleUndoLike}
          viewType={viewType}
        />
      );
  };

  const handleLike = async (targetUserId: string) => {
    try {
      await api.post('/Recommendation/like', { requestedUserId: targetUserId });
      onInteraction(targetUserId);
    } catch (err) {
      console.error("Error liking user", err);
    }
    setOpen(false);
  };

  const handleDislike = async (targetUserId: string) => {
    try {
      await api.post('/Recommendation/dislike', { requestedUserId: targetUserId });
      onInteraction(targetUserId);
    } catch (err) {
      console.error("Error disliking user", err);
    }
    setOpen(false);
  };

 const handleUndoLike = async (id: string) => {
    try {
      await api.post('/Likes/undoLike', { requestedUserId: id });
      onInteraction(id);
    } catch (err) { console.error("Error undoing like", err); }
    setOpen(false);
  };

  const handleUndoDislike = async (id: string) => {
    try {
      await api.post('/Likes/undoDislike', { requestedUserId: id });
      onInteraction(id);
    } catch (err) { console.error("Error undoing dislike", err); }
    setOpen(false);
  };

  return (
    <React.Fragment>
      <ButtonBase onClick={() => setOpen(true)}>
        {renderSmallProfile()}
      </ButtonBase>
      <Dialog open={open} onClose={() => setOpen(false)} scroll="paper" aria-labelledby="scroll-dialog-title" aria-describedby="scroll-dialog-description"     >
        <DetailedProfile profile={profile} handleLike={handleLike} handleDislike={handleDislike} handleUndoDislike={handleUndoDislike} handleUndoLike={handleUndoLike}
          viewType={viewType} open={open} />
      </Dialog>
    </React.Fragment>
  );
}
