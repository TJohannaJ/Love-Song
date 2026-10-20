import { Box, Typography, IconButton, Tooltip } from "@mui/material";
import CloseIcon from "@mui/icons-material/Close";
import DeleteSweepIcon from "@mui/icons-material/DeleteSweep";

interface AiChatHeaderProps {
    onClose: () => void;
    onClearHistory?: () => void; // <-- Add hozzá ezt a sort!
}

export function AiChatHeader({ onClose, onClearHistory }: AiChatHeaderProps) {
    return (
        <Box
            sx={{
                p: 2,
                background: "linear-gradient(135deg, #FF007A 0%, #FF5E00 100%)",
                color: "#ffffff",
                display: "flex",
                alignItems: "center",
                justifyContent: "space-between"
            }}
        >
            <Typography variant="subtitle1" fontWeight="bold">
                LoveSong AI
            </Typography>
            <Box sx={{ display: "flex", alignItems: "center", gap: 0.5 }}>
                {onClearHistory && (
                    <Tooltip title="Clear chat history">
                        <IconButton size="small" onClick={onClearHistory} sx={{ color: "#ffffff" }}>
                            <DeleteSweepIcon fontSize="small" />
                        </IconButton>
                    </Tooltip>
                )}
                <IconButton size="small" onClick={onClose} sx={{ color: "#ffffff" }}>
                    <CloseIcon fontSize="small" />
                </IconButton>
            </Box>
        </Box>
    );
}