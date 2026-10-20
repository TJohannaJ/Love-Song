import api from '../api/axiosInstance';
import { ChatMessage } from "../intefaces/iChatMessage";
import { Profile } from "../intefaces/iProfile";

export interface ChatRequest {
    history: ChatMessage[];
}

export interface ChatResponse {
    reply: string;
    profiles?: Profile[]; // <-- ÚJ: A backendről érkező profilok
}

export const aiService = {
    sendMessage: (history: ChatMessage[]) =>
        api.post<ChatResponse>('/AiChat/message', { history }),
};