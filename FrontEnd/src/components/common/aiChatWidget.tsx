import { useState } from "react";
import { Fab, Paper, Box } from "@mui/material";
import AutoAwesomeIcon from "@mui/icons-material/AutoAwesome";
import { aiService } from '../../services/aiService';
import { socialService } from '../../services/socialService'; // <-- A pontos szerviz importálása!
import { ChatMessage } from "../../intefaces/iChatMessage";
import { AiChatHeader } from "./aiChatHeader";
import { AiChatMessageList } from "./aiChatMessageList";
import { AiChatInput } from "./aiChatInput";
import { useAiChatHistory } from "../../hooks/useAiChatHistory";

export function AiChatWidget() {
    const [open, setOpen] = useState(false);
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
            console.error("AI Chat frontend error:", error);
            setMessages(prev => [
                ...prev,
                { role: "assistant", content: "Sorry, an error occurred while generating the response. Please try again!" }
            ]);
        } finally {
            setLoading(false);
        }
    };

    // LIKE / DISLIKE KEZELÉSE A WIDGETBEN A SOCIALSERVICE HÍVÁSOKKAL
    const handleProfileInteraction = async (profileId: string, type: 'like' | 'dislike' | 'undo') => {
        try {
            if (type === 'like') {
                await socialService.likeRecommendation(profileId);
            } else if (type === 'dislike') {
                await socialService.dislikeRecommendation(profileId);
            } else if (type === 'undo') {
                await socialService.undoDislike(profileId);
            }

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
        <Box sx={{ position: "fixed", bottom: 24, right: 24, zIndex: 1300 }}>
            {!open && (
                <Fab
                    onClick={() => setOpen(true)}
                    sx={{
                        background: "linear-gradient(135deg, #FF007A 0%, #FF5E00 100%)",
                        color: "#fff",
                        boxShadow: "0 4px 20px rgba(255, 0, 122, 0.4)",
                        "&:hover": {
                            background: "linear-gradient(135deg, #E0006C 0%, #E05300 100%)",
                            transform: "scale(1.05)",
                        },
                        transition: "all 0.2s ease-in-out"
                    }}
                >
                    <AutoAwesomeIcon fontSize="medium" />
                </Fab>
            )}

            {open && (
                <Paper
                    elevation={12}
                    sx={{
                        width: { xs: "320px", sm: "380px" },
                        height: "480px",
                        borderRadius: "16px",
                        display: "flex",
                        flexDirection: "column",
                        overflow: "hidden",
                        border: "1px solid rgba(255, 255, 255, 0.12)",
                        boxShadow: "0 8px 32px rgba(0, 0, 0, 0.3)"
                    }}
                >
                    <AiChatHeader onClose={() => setOpen(false)} onClearHistory={clearHistory} />
                    <AiChatMessageList
                        messages={messages}
                        loading={loading}
                        onProfileInteraction={handleProfileInteraction}
                    />
                    <AiChatInput onSend={handleSend} loading={loading} />
                </Paper>
            )}
        </Box>
    );
}