import { useEffect, useRef } from "react";
import { Box, Typography, CircularProgress, Paper, List, Divider, useTheme } from "@mui/material";
import ReactMarkdown from "react-markdown";
import { ChatMessage } from "../../intefaces/iChatMessage";
import ClickableProfileCard from "../../components/common/clickableProfileCard";

interface AiChatMessageListProps {
    messages: ChatMessage[];
    loading: boolean;
    onProfileInteraction?: (id: string, type: 'like' | 'dislike' | 'undo') => void;
}

export function AiChatMessageList({ messages, loading, onProfileInteraction }: AiChatMessageListProps) {
    const messagesEndRef = useRef<HTMLDivElement>(null);
    const theme = useTheme();

    const scrollToBottom = () => {
        messagesEndRef.current?.scrollIntoView({ behavior: "smooth" });
    };

    useEffect(() => {
        scrollToBottom();
    }, [messages, loading]);

    const handleInteraction = (id: string, type: 'like' | 'dislike' | 'undo') => {
        if (onProfileInteraction) {
            onProfileInteraction(id, type);
        }
    };

    return (
        <Box
            sx={{
                flexGrow: 1,
                p: 2.5,
                overflowY: "auto",
                display: "flex",
                flexDirection: "column",
                gap: 2,
                backgroundColor: theme.palette.mode === 'dark' ? '#0a0e1a' : '#f1f2f4',
            }}
        >
            {messages.map((msg, index) => (
                <Box
                    key={index}
                    sx={{
                        alignSelf: msg.role === "user" ? "flex-end" : "flex-start",
                        maxWidth: msg.profiles && msg.profiles.length > 0 ? "90%" : "80%",
                        width: msg.profiles && msg.profiles.length > 0 ? "100%" : "auto",
                        display: "flex",
                        flexDirection: "column",
                        gap: 1.5
                    }}
                >
                    {/* AI / Felhasználó szöveges buboréka */}
                    {msg.content && (
                        <Box
                            sx={{
                                p: 1.75,
                                borderRadius: "18px",
                                background: msg.role === "user"
                                    ? `linear-gradient(135deg, ${theme.palette.primary.main} 0%, ${theme.palette.primary.light} 100%)`
                                    : theme.palette.background.paper,
                                color: msg.role === "user" ? theme.palette.primary.contrastText : theme.palette.text.primary,
                                boxShadow: theme.palette.mode === 'dark'
                                    ? "0 4px 12px rgba(0, 0, 0, 0.4)"
                                    : "0 2px 8px rgba(0, 0, 0, 0.06)",
                                fontSize: "0.95rem",
                                lineHeight: 1.5,
                                wordBreak: "break-word",
                                border: msg.role === "user" ? "none" : `1px solid ${theme.palette.divider}`,
                                "& p": { margin: 0 }
                            }}
                        >
                            <ReactMarkdown>{msg.content}</ReactMarkdown>
                        </Box>
                    )}

                    {/* Interaktív NotificationProfile kártyák listája */}
                    {msg.profiles && msg.profiles.length > 0 && (
                        <Paper
                            elevation={2}
                            sx={{
                                borderRadius: 3,
                                overflow: 'hidden',
                                border: `1px solid ${theme.palette.divider}`,
                                backgroundColor: theme.palette.background.paper
                            }}
                        >
                            <List sx={{ p: 0, width: '100%' }}>
                                {msg.profiles.map((profile, pIdx) => (
                                    <Box key={profile.id}>
                                        <ClickableProfileCard
                                            profile={profile}
                                            onInteraction={(id, type) => handleInteraction(id, type)}
                                            viewType="notification" // <-- A kívánt NotificationProfile nézet[cite: 35, 36]
                                        />
                                        {pIdx < msg.profiles!.length - 1 && <Divider variant="fullWidth" component="li" />}
                                    </Box>
                                ))}
                            </List>
                        </Paper>
                    )}
                </Box>
            ))}

            {loading && (
                <Box sx={{ alignSelf: "flex-start", display: "flex", alignItems: "center", gap: 1, p: 1 }}>
                    <CircularProgress size={18} color="primary" />
                    <Typography variant="caption" color="text.secondary">
                        LoveSong Assistant is typing...
                    </Typography>
                </Box>
            )}
            <div ref={messagesEndRef} />
        </Box>
    );
}