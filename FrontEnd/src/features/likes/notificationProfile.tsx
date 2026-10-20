import { Avatar, ListItem, ListItemAvatar, ListItemText, Typography, Box } from "@mui/material";
import React from "preact/compat";
import { Profile } from "../../intefaces/iProfile";
import { DislikeButton } from "../../components/common/dislikeButton";
import { LikeButton } from "../../components/common/likeButton";
import { UndoButton } from "../../components/common/undoButton";
import { ProfileViewType } from "../../components/common/clickableProfileCard";

interface NotificationProfileProps {
  profile: Profile;
  handleLike?: (id: string) => void;
  handleDislike?: (id: string) => void;
  handleUndoLike?: (id: string) => void;
  handleUndoDislike?: (id: string) => void;
  viewType: ProfileViewType;
}

export function NotificationProfile({ profile, handleLike, handleDislike, handleUndoLike, handleUndoDislike, viewType }: NotificationProfileProps) {
  return (
    <ListItem
      alignItems="flex-start"
      sx={{
        py: 2,
        width: '100%',
        transition: '0.3s ease',
        boxShadow: 'none',
        '&:hover': {
          backgroundColor: 'rgba(0, 0, 0, 0.02)',
          boxShadow: '0 4px 14px rgba(47, 48, 53, 0.34)',
          position: 'relative',
          zIndex: 1,
        }
      }}
    >
        <ListItemAvatar>
            <Avatar
                alt={profile.fullName}
                src={profile.pictureUrl || '/public/profile-placeholder.png'} // Base64 prefix eltávolítva
                sx={{ width: 56, height: 56, mr: 2 }}
            />
        </ListItemAvatar>

      <ListItemText
        sx={{ minWidth: 0, flex: 1 }}
        primary={
          <Typography variant="h6" component="div">
            {profile.fullName}, {profile.age}
          </Typography>
        }
        secondary={
          <React.Fragment>
            <Typography
              component="span"
              variant="body2"
              sx={{ color: 'text.primary', display: 'inline', fontWeight: 'bold' }}
            />
            {`${profile.shortIntro}`}
          </React.Fragment>
        }
      />
      <Box sx={{ display: 'flex', flexWrap: 'nowrap', flexShrink: 0, gap: 1, alignSelf: 'center', ml: 2 }}>
        {viewType === 'likeHistory' ? (
          <UndoButton profile={profile} handleUndo={handleUndoLike} />
        ) : viewType === 'dislikeHistory' ? (
          <UndoButton profile={profile} handleUndo={handleUndoDislike} />
        ) : (
          <>
            <DislikeButton profile={profile} handleDislike={handleDislike} />
            <LikeButton profile={profile} handleLike={handleLike} />
          </>
        )}
      </Box>
    </ListItem>
  );
}