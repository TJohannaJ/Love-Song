import {
    Box, TextField, Button, Typography,
    CircularProgress, MenuItem, Dialog,
    DialogTitle, DialogContent, DialogActions, Chip, Stack
} from "@mui/material";
import { useState } from "react";
import { userService } from "../../services/userService";

interface AiBioDialogProps {
    open: boolean;
    onClose: () => void;
    onSuccess: (shortIntro: string, longIntro: string) => void;
    onError: (msg: string) => void;
}

export function AiBioDialog({ open, onClose, onSuccess, onError }: AiBioDialogProps) {
    const [aiLoading, setAiLoading] = useState(false);
    const [tone, setTone] = useState("playful");
    const [keywordInput, setKeywordInput] = useState("");
    const [keywords, setKeywords] = useState<string[]>([]);
    const [extraContext, setExtraContext] = useState("");

    const handleAddKeyword = (e: React.KeyboardEvent<HTMLInputElement>) => {
        if ((e.key === 'Enter' || e.key === ',') && keywordInput.trim()) {
            e.preventDefault();
            e.stopPropagation();
            if (!keywords.includes(keywordInput.trim())) {
                setKeywords([...keywords, keywordInput.trim()]);
            }
            setKeywordInput("");
        }
    };

    const handleDeleteKeyword = (chipToDelete: string) => {
        setKeywords(keywords.filter(k => k !== chipToDelete));
    };

    const handleGenerateAiBio = async () => {
        setAiLoading(true);
        try {
            const response = await userService.generateBio({
                keywords,
                tone,
                extraContext
            });

            if (response && response.data) {
                const data = response.data as any;
                const shortBio = data.shortIntro || data.shortintro || data.ShortIntro || "";
                const longBio = data.longIntro || data.longintro || data.LongIntro || "";

                if (!shortBio && !longBio) {
                    throw new Error("Invalid response format from AI");
                }

                onSuccess(shortBio, longBio);
                onClose();
            }
        } catch (error: any) {
            console.error("AI Bio Error:", error);
            onError(error.response?.data?.message || error.message || "An error occurred during AI bio generation.");
        } finally {
            setAiLoading(false);
        }
    };

    return (
        <Dialog open={open} onClose={() => !aiLoading && onClose()} maxWidth="sm" fullWidth>
            <DialogTitle sx={{ fontWeight: 'bold' }}>✨ AI Bio Assistant</DialogTitle>
            <DialogContent dividers>
                <Typography variant="body2" color="textSecondary" mb={2}>
                    Set your preferences, and AI will craft both a short and long introduction tailored to your profile.
                </Typography>

                <Box display="flex" flexDirection="column" gap={2}>
                    <TextField
                        select
                        fullWidth
                        label="Tone / Style"
                        value={tone}
                        onChange={(e) => setTone((e.target as HTMLInputElement).value)}
                    >
                        <MenuItem value="playful">Playful / Flirty 😉</MenuItem>
                        <MenuItem value="witty">Witty / Funny 😂</MenuItem>
                        <MenuItem value="casual">Casual / Easygoing 😎</MenuItem>
                        <MenuItem value="romantic">Deep / Romantic ✨</MenuItem>
                        <MenuItem value="mysterious">Mysterious / Intrigued 🌙</MenuItem>
                    </TextField>

                    <TextField
                        fullWidth
                        label="Interests & Keywords"
                        placeholder="Type a word and press Enter (e.g., cooking, coffee, travel)"
                        value={keywordInput}
                        onChange={(e) => setKeywordInput((e.target as HTMLInputElement).value)}
                        onKeyDown={handleAddKeyword}
                    />

                    {keywords.length > 0 && (
                        <Stack direction="row" spacing={1} flexWrap="wrap" useFlexGap gap={1}>
                            {keywords.map((k) => (
                                <Chip key={k} label={k} onDelete={() => handleDeleteKeyword(k)} color="primary" variant="outlined" />
                            ))}
                        </Stack>
                    )}

                    <TextField
                        fullWidth
                        multiline
                        rows={2}
                        label="Special Request (Optional)"
                        placeholder="e.g., 'Mention that I have a cat'"
                        value={extraContext}
                        onChange={(e) => setExtraContext((e.target as HTMLTextAreaElement).value)}
                    />
                </Box>
            </DialogContent>

            <DialogActions sx={{ p: 2 }}>
                <Button onClick={onClose} disabled={aiLoading}>
                    Cancel
                </Button>
                <Button
                    onClick={handleGenerateAiBio}
                    disabled={aiLoading}
                    variant="contained"
                    startIcon={aiLoading ? <CircularProgress size={20} color="inherit" /> : null}
                    sx={{
                        px: 4,
                        py: 1.2,
                        borderRadius: '25px',
                        fontWeight: 'bold',
                        color: '#ffffff',
                        background: 'linear-gradient(135deg, #FF007A 0%, #FF5E00 100%)',
                        boxShadow: '0 4px 14px rgba(255, 0, 122, 0.4)',
                        transition: 'all 0.2s ease-in-out',
                        '&:hover': {
                            background: 'linear-gradient(135deg, #E0006C 0%, #E05300 100%)',
                            boxShadow: '0 6px 18px rgba(255, 0, 122, 0.55)',
                            transform: 'translateY(-1px)',
                        },
                        '&.Mui-disabled': {
                            background: 'rgba(255, 255, 255, 0.12)',
                            color: 'rgba(255, 255, 255, 0.3)',
                        }
                    }}
                >
                    {aiLoading ? "Generating..." : "Generate with AI"}
                </Button>
            </DialogActions>
        </Dialog>
    );
}