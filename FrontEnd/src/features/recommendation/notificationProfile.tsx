import { Avatar, ListItem, ListItemAvatar, ListItemText, Typography, IconButton, Box } from "@mui/material";
import React from "preact/compat";
import { Profile } from "../../intefaces/iProfile";
import { DislikeButton } from "../../components/common/dislikeButton";
import { LikeButton } from "../../components/common/likeButton";
import { Undo } from "@mui/icons-material";
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
        borderRadius: 1,
        '&:hover': {
          boxShadow: 6,
          backgroundColor: 'rgba(0, 0, 0, 0.02)'
        }
      }}
    >
      <ListItemAvatar>
        <Avatar
          alt={profile.fullName}
          src={`data:image/png;base64,${profile.picture}`}
          sx={{ width: 56, height: 56, mr: 2 }}
        />
      </ListItemAvatar>

      <ListItemText
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
      <Box sx={{ display: 'flex', gap: 1, alignSelf: 'center', ml: 2 }}>
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