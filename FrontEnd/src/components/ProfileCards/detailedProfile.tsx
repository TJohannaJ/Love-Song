import { Button, CardMedia, DialogActions, DialogContent, DialogContentText, DialogTitle } from "@mui/material";
import React from "preact/compat";
import { Profile } from "../../intefaces/iProfile";
import { DislikeButton } from "../dislikeButton";
import { LikeButton } from "../likeButton";

export function DetailedProfile({ profile, handleLike, handleDislike, open }: { profile: Profile, handleLike: (id: string) => void, handleDislike: (id: string) => void, open: boolean }) {
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
                <CardMedia component="img" alt="Profile Picture" height="%100" image={`data:image/png;base64,${profile.picture}`} />
                {profile.longIntro}
            </DialogContentText>
        </DialogContent>
        <DialogActions>
            <DislikeButton profile={profile} handleDislike={handleDislike} />
            <LikeButton profile={profile} handleLike={handleLike} />  
        </DialogActions></>
}