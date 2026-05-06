export interface Message {
    id: string;
    content: string;
    senderId: string;
    createdAt: string; // Ha a backend küldi, érdemes felvenni
}