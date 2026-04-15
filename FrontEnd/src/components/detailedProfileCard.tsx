import { Button, CardMedia, DialogActions, DialogContent, DialogContentText, DialogTitle } from "@mui/material";
import { ProfileCardProps } from "./profilecard";
import React from "preact/compat";

export function DetailedProfileCard({ profile, handleLike, handleDislike, open }: { profile: ProfileCardProps, handleLike: (id: string) => void, handleDislike: (id: string) => void, open: boolean }) {
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
            <Button onClick={() => handleLike(profile.id)}>Like</Button>
            <Button onClick={() => handleDislike(profile.id)}>Dislike</Button>
        </DialogActions></>
}