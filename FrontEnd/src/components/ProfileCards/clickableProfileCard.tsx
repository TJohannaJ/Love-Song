import * as React from 'react';
import ButtonBase from '@mui/material/ButtonBase';
import Dialog from '@mui/material/Dialog';
import api from '../../api/axiosInstance';
import { LittleProfile } from './littleProfile';
import { DetailedProfile } from './detailedProfile';
import { Profile } from '../../intefaces/iProfile';
import { NotificationProfile } from './notificationProfile';

type ProfileViewType = 'notification' | 'little' | 'history';

export interface ProfileCardProps {
  profile: Profile;
  onInteraction: (id: string) => void;
  viewType: ProfileViewType;
}

export default function ClickableProfileCard({ profile, onInteraction, viewType }: ProfileCardProps) {
  const [open, setOpen] = React.useState(false);
  
  const renderSmallProfile = () => {
        if (viewType === 'notification') {
            return <NotificationProfile profile={profile} />;
        }
        return <LittleProfile profile={profile} handleLike={handleLike} handleDislike={handleDislike} />;
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

  return (
    <React.Fragment>
      <ButtonBase onClick={() => setOpen(true)}>
        {renderSmallProfile()}
      </ButtonBase>
      <Dialog open={open} onClose={() => setOpen(false)} scroll="paper" aria-labelledby="scroll-dialog-title" aria-describedby="scroll-dialog-description"     >
        <DetailedProfile profile={profile} handleLike={handleLike} handleDislike={handleDislike} open={open} />
      </Dialog>
    </React.Fragment>
  );
}
