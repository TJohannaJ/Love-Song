import * as React from 'react';
import { HubConnection, HubConnectionBuilder, LogLevel } from '@microsoft/signalr';
import { useAuth } from '../features/auth/authContext';
import { chatService } from '../services/chatService';

export const useChat = () => {
    const { token } = useAuth();
    const [matches, setMatches] = React.useState<any[]>([]);
    const [selectedMatchId, setSelectedMatchId] = React.useState('');
    const [messages, setMessages] = React.useState<Record<string, any[]>>({});
    const [connection, setConnection] = React.useState<HubConnection | null>(null);
    const [loading, setLoading] = React.useState(true);

    // Matchek betöltése
    React.useEffect(() => {
        if (token) {
            chatService.getMatches().then(res => {
                setMatches(res.data);
                setLoading(false);
            });
        }
    }, [token]);

    // Üzenetek betöltése partnerváltáskor
    React.useEffect(() => {
        if (!selectedMatchId || !token) return;
        if (!messages[selectedMatchId]) {
            chatService.getConversation(selectedMatchId).then(res => {
                const history = res.data.map((m: any) => ({
                    id: m.id,
                    text: m.content,
                    author: m.senderId.toLowerCase() === selectedMatchId.toLowerCase() ? 'them' : 'me'
                }));
                setMessages(prev => ({ ...prev, [selectedMatchId]: history }));
            });
        }
    }, [selectedMatchId, token]);

    // SignalR kapcsolat
    React.useEffect(() => {
        if (!token || connection) return;
        const newConnection = new HubConnectionBuilder()
            .withUrl(chatService.hubUrl, { accessTokenFactory: () => token })
            .withAutomaticReconnect()
            .build();
        setConnection(newConnection);
        return () => { if (newConnection) newConnection.stop(); };
    }, [token]);

    // Eseménykezelők
    React.useEffect(() => {
        if (!connection) return;
        if (connection.state === "Disconnected") connection.start().catch(console.error);

        const updateMatchList = (partnerId: string, text: string, isMe: boolean) => {
            setMatches(prev => prev.map(m => 
                m.id.toLowerCase() === partnerId.toLowerCase() 
                ? { ...m, lastMessage: text, lastMessageSenderName: isMe ? "Te" : "Partner" } 
                : m
            ));
        };

        const handleReceive = (msg: any) => {
            const partnerId = msg.senderId;
            setMessages(prev => {
                const chat = prev[partnerId] || [];
                if (chat.some(m => m.id === msg.id)) return prev;
                return { ...prev, [partnerId]: [...chat, { author: 'them', text: msg.content, id: msg.id }] };
            });
            updateMatchList(partnerId, msg.content, false);
        };

        const handleSent = (msg: any) => {
            updateMatchList(msg.receiverId, msg.content, true);
        };

        connection.on("ReceiveMessage", handleReceive);
        connection.on("MessageSent", handleSent);
        return () => {
            connection.off("ReceiveMessage");
            connection.off("MessageSent");
        };
    }, [connection]);

    const sendMessage = async (text: string) => {
        if (!connection || !selectedMatchId) return;
        const tempId = Date.now().toString();
        
        // Lokális UI frissítés azonnal
        setMessages(prev => ({
            ...prev,
            [selectedMatchId]: [...(prev[selectedMatchId] || []), { author: 'me', text, id: tempId }]
        }));
        
        // Lista frissítése azonnal
        setMatches(prev => prev.map(m => 
            m.id.toLowerCase() === selectedMatchId.toLowerCase() 
            ? { ...m, lastMessage: text, lastMessageSenderName: "Te" } 
            : m
        ));

        await connection.invoke("SendMessage", { receiverId: selectedMatchId, content: text });
    };

    return { matches, selectedMatchId, setSelectedMatchId, messages, loading, sendMessage };
};