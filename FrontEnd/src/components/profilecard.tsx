import * as React from 'react';
import ButtonBase from '@mui/material/ButtonBase';
import Dialog from '@mui/material/Dialog';
import { Profile } from '../intefaces/profile';
import api from '../api/axiosInstance';
import { LittleProfileCard } from './littleProfileCard';
import { DetailedProfileCard } from './detailedProfileCard';

export interface ProfileCardProps extends Profile {
  onInteraction: (id: string) => void;
}

export default function ProfileCard(profile: ProfileCardProps) {
  const [open, setOpen] = React.useState(false);

  const handleLike = async (targetUserId: string) => {
    try {
      await api.post('/Recommendation/like', { likedUserId: targetUserId });
      profile.onInteraction(targetUserId);
    } catch (err) {
      console.error("Error liking user", err);
    }
    setOpen(false);
  };

  const handleDislike = async (targetUserId: string) => {
    try {
      await api.post('/Recommendation/dislike', { dislikedUserId: targetUserId });
      profile.onInteraction(targetUserId);
    } catch (err) {
      console.error("Error disliking user", err);
    }
    setOpen(false);
  };

  return (
    <React.Fragment>
      <ButtonBase onClick={() => setOpen(true)}>
        <LittleProfileCard profile={profile} handleLike={() => handleLike(profile.id)} handleDislike={() => handleDislike(profile.id)} />
      </ButtonBase>
      <Dialog open={open} onClose={() => setOpen(false)} scroll="paper" aria-labelledby="scroll-dialog-title" aria-describedby="scroll-dialog-description"     >
        <DetailedProfileCard profile={profile} handleLike={handleLike} handleDislike={handleDislike} open={open} />
      </Dialog>
    </React.Fragment>
  );
}
