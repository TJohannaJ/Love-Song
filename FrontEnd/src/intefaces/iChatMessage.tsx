import { Profile } from './iProfile';

export interface ChatMessage {
    role: 'user' | 'assistant';
    content: string;
    profiles?: Profile[]; 
}