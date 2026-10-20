import { Button, CardMedia, DialogActions, DialogContent, DialogContentText, DialogTitle } from "@mui/material";
import React from "preact/compat";
import { Profile } from "../../intefaces/iProfile";
import { DislikeButton } from "./dislikeButton";
import { LikeButton } from "./likeButton";
import { UndoButton } from "./undoButton";
import { ProfileViewType } from "./clickableProfileCard";

interface DetailedProfileProps {
  profile: Profile;
  handleLike: (id: string) => void;
  handleDislike: (id: string) => void;
  handleUndoLike: (id: string) => void,
  handleUndoDislike: (id: string) => void,
  viewType: ProfileViewType;              
  open: boolean;
}

export function DetailedProfile({ profile, handleLike, handleDislike, handleUndoLike, handleUndoDislike, viewType, open }: DetailedProfileProps) {
    const descriptionElementRef = React.useRef<HTMLElement>(null);
    React.useEffect(() => {
        if (open) {
            const { current: descriptionElement } = descriptionElementRef;
            if (descriptionElement !== null) {
                descriptionElement.focus();
            }
        }
    }, [open]);

    return <><DialogTitle id="scroll-dialog-title">{profile.fullName}, {profile.age}</DialogTitle>
        <DialogContent dividers={true}>
            <DialogContentText id="scroll-dialog-description" ref={descriptionElementRef} tabIndex={-1}>
                <CardMedia
                    component="img"
                    alt="Profile Picture"
                    height="100%"
                    image={profile.picture || '/default-profile.png'} // Base64 prefix eltávolítva
                />
                {profile.longIntro}
            </DialogContentText>
        </DialogContent>
        <DialogActions>
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
        </DialogActions></>
}