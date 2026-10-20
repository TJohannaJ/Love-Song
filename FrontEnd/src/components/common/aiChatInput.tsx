import { useState } from "react";
import { Box, TextField, IconButton, useTheme } from "@mui/material";
import SendIcon from "@mui/icons-material/Send";

interface AiChatInputProps {
    onSend: (message: string) => void;
    loading: boolean;
    isFullPage?: boolean; // <-- ÚJ PROP
}

export function AiChatInput({ onSend, loading, isFullPage = false }: AiChatInputProps) {
    const [input, setInput] = useState("");
    const theme = useTheme();

    const handleSubmit = (e: React.FormEvent) => {
        e.preventDefault();
        if (!input.trim() || loading) return;

        onSend(input);
        setInput("");
    };

    return (
        <Box
            component="form"
            onSubmit={handleSubmit}
            sx={{
                p: 2,
                display: "flex",
                gap: 1.5,
                alignItems: "center",
                backgroundColor: theme.palette.background.paper,
                // Ha teljes oldalon van, mind a 4 sarka kerekített, egyébként csak az alja
                borderRadius: isFullPage ? "16px" : "0 0 16px 16px",
                borderTop: isFullPage ? "none" : `1px solid ${theme.palette.divider}`,
            }}
        >
            <TextField
                fullWidth
                size="small"
                placeholder="Ask something about the app..."
                value={input}
                onChange={(e) => setInput((e.target as HTMLInputElement).value)}
                disabled={loading}
                variant="outlined"
                sx={{
                    "& .MuiOutlinedInput-root": {
                        borderRadius: "24px",
                        backgroundColor: theme.palette.mode === 'dark' ? '#0a0e1a' : '#f8f9fa',
                        "& fieldset": {
                            borderColor: theme.palette.mode === 'dark' ? 'rgba(255, 255, 255, 0.15)' : 'rgba(0, 0, 0, 0.12)',
                        },
                        "&:hover fieldset": {
                            borderColor: theme.palette.primary.main,
                        }
                    }
                }}
            />
            <IconButton
                type="submit"
                disabled={!input.trim() || loading}
                sx={{
                    width: 40,
                    height: 40,
                    background: `linear-gradient(135deg, ${theme.palette.primary.main} 0%, ${theme.palette.primary.light} 100%)`,
                    color: "#ffffff",
                    boxShadow: "0 2px 8px rgba(255, 0, 122, 0.3)",
                    "&:hover": {
                        background: `linear-gradient(135deg, ${theme.palette.primary.dark} 0%, ${theme.palette.primary.main} 100%)`,
                    },
                    "&.Mui-disabled": {
                        background: theme.palette.mode === 'dark' ? 'rgba(255,255,255,0.08)' : 'rgba(0,0,0,0.12)',
                        color: theme.palette.mode === 'dark' ? 'rgba(255,255,255,0.3)' : 'rgba(0,0,0,0.26)',
                        boxShadow: "none"
                    }
                }}
            >
                <SendIcon fontSize="small" />
            </IconButton>
        </Box>
    );
}