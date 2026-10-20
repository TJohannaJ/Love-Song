export interface Match {
    id: string;
    fullName: string;
    age: number;
    pictureUrl: string;
    lastMessage: string;
    lastMessageTime: string; // ISO formátumú időbélyeg
    lastMessageSenderName: string; // Az utolsó üzenet küldőjének neve
}