export interface Match {
    id: string;
    fullName: string;
    age: number;
    picture: string; // Base64 kódolt kép
    lastMessage: string;
    lastMessageTime: string; // ISO formátumú időbélyeg
}