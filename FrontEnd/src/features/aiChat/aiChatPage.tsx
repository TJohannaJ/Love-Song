import { useState } from "react";
import { Box, Paper, Typography, Avatar, Container, useTheme, IconButton, Tooltip } from "@mui/material";
import AutoAwesomeIcon from "@mui/icons-material/AutoAwesome";
import DeleteSweepIcon from "@mui/icons-material/DeleteSweep";
import { aiService } from "../../services/aiService";
import { socialService } from "../../services/socialService"; // <-- A pontos szerviz importálása!
import { ChatMessage } from "../../intefaces/iChatMessage";
import { AiChatMessageList } from "../../components/common/aiChatMessageList";
import { AiChatInput } from "../../components/common/aiChatInput";
import { useAiChatHistory } from "../../hooks/useAiChatHistory";

export function AiChatPage() {
    const theme = useTheme();
    const { messages, setMessages, clearHistory } = useAiChatHistory();
    const [loading, setLoading] = useState(false);

    const handleSend = async (inputText: string) => {
        const userMsg: ChatMessage = { role: "user", content: inputText };
        const updatedHistory = [...messages, userMsg];

        setMessages(updatedHistory);
        setLoading(true);

        try {
            const response = await aiService.sendMessage(updatedHistory);

            if (response.data && response.data.reply) {
                setMessages(prev => [
                    ...prev,
                    {
                        role: "assistant",
                        content: response.data.reply,
                        profiles: response.data.profiles
                    }
                ]);
            }
        } catch (error) {
            console.error("AI Chat error:", error);
            setMessages(prev => [
                ...prev,
                { role: "assistant", content: "Sorry, an error occurred while generating the response. Please try again!" }
            ]);
        } finally {
            setLoading(false);
        }
    };

    // LIKE / DISLIKE KEZELÉSE A SOCIALSERVICE HÍVÁSOKKAL
    const handleProfileInteraction = async (profileId: string, type: 'like' | 'dislike' | 'undo') => {
        try {
            if (type === 'like') {
                await socialService.likeRecommendation(profileId);
            } else if (type === 'dislike') {
                await socialService.dislikeRecommendation(profileId);
            } else if (type === 'undo') {
                await socialService.undoDislike(profileId);
            }

            // Profil kártya eltávolítása az üzenetből az interakció után
            setMessages(prev => prev.map(msg => {
                if (msg.profiles) {
                    return {
                        ...msg,
                        profiles: msg.profiles.filter(p => p.id !== profileId)
                    };
                }
                return msg;
            }));
        } catch (error) {
            console.error(`Error performing ${type} on user ${profileId}:`, error);
        }
    };

    return (
        <Container
            maxWidth="lg"
            disableGutters
            sx={{ height: "100%", display: "flex", flexDirection: "column", gap: 2 }}
        >
            <Paper
                elevation={0}
                sx={{
                    px: 3,
                    py: 2,
                    borderRadius: "16px",
                    background: `linear-gradient(135deg, ${theme.palette.primary.main} 0%, ${theme.palette.primary.light} 100%)`,
                    color: "#ffffff",
                    display: "flex",
                    alignItems: "center",
                    justifyContent: "space-between",
                    border: "none",
                    flexShrink: 0
                }}
            >
                <Box sx={{ display: "flex", alignItems: "center", gap: 2 }}>
                    <Avatar sx={{ bgcolor: "rgba(255,255,255,0.2)", width: 44, height: 44 }}>
                        <AutoAwesomeIcon sx={{ color: "#ffffff" }} fontSize="medium" />
                    </Avatar>
                    <Box>
                        <Typography variant="h6" fontWeight="bold" lineHeight={1.2}>
                            LoveSong AI Assistant
                        </Typography>
                        <Typography variant="body2" sx={{ opacity: 0.9, fontWeight: 300 }}>
                            Ask about matches, recommendations, or app features
                        </Typography>
                    </Box>
                </Box>

                <Tooltip title="Clear chat history">
                    <IconButton
                        onClick={clearHistory}
                        sx={{
                            color: "#ffffff",
                            backgroundColor: "rgba(255,255,255,0.15)",
                            "&:hover": { backgroundColor: "rgba(255,255,255,0.25)" }
                        }}
                    >
                        <DeleteSweepIcon />
                    </IconButton>
                </Tooltip>
            </Paper>

            <Paper
                elevation={0}
                sx={{
                    flexGrow: 1,
                    display: "flex",
                    flexDirection: "column",
                    borderRadius: "16px",
                    overflow: "hidden",
                    border: "none",
                    backgroundColor: theme.palette.background.paper,
                    minHeight: 0
                }}
            >
                <AiChatMessageList
                    messages={messages}
                    loading={loading}
                    onProfileInteraction={handleProfileInteraction}
                />
                <AiChatInput onSend={handleSend} loading={loading} isFullPage />
            </Paper>
        </Container>
    );
}