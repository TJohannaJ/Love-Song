import api from '../api/axiosInstance';
import { Match } from '../intefaces/iMatch';

export const chatService = {
    getMatches: () => api.get<Match[]>("/Chat/matches"),
    getConversation: (otherUserId: string) => 
        api.get(`/Chat/conversation/${otherUserId}`),
    hubUrl: "https://localhost:7160/chathub"
};