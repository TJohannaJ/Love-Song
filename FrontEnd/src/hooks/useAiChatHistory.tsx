import { useState, useEffect } from "react";
import { ChatMessage } from "../intefaces/iChatMessage";

const STORAGE_KEY = "lovesong_ai_chat_history";

const INITIAL_MESSAGE: ChatMessage = {
    role: "assistant",
    content: "Hi! I am LoveSong's AI Assistant. How can I help you find matches or navigate the app today?"
};

export function useAiChatHistory() {
    const [messages, setMessages] = useState<ChatMessage[]>(() => {
        try {
            const saved = sessionStorage.getItem(STORAGE_KEY);
            return saved ? JSON.parse(saved) : [INITIAL_MESSAGE];
        } catch (error) {
            console.error("Failed to load chat history from sessionStorage:", error);
            return [INITIAL_MESSAGE];
        }
    });

    useEffect(() => {
        try {
            sessionStorage.setItem(STORAGE_KEY, JSON.stringify(messages));
        } catch (error) {
            console.error("Failed to save chat history to sessionStorage:", error);
        }
    }, [messages]);

    const clearHistory = () => {
        setMessages([INITIAL_MESSAGE]);
        sessionStorage.removeItem(STORAGE_KEY);
    };

    return { messages, setMessages, clearHistory };
}