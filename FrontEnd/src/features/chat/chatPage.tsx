import * as React from 'react';
import {
    Avatar, Box, Divider, IconButton, InputAdornment, List,
    ListItem, ListItemAvatar, ListItemButton, ListItemText,
    Paper, TextField, Typography
} from '@mui/material';
import SendIcon from '@mui/icons-material/Send';
import { HubConnection, HubConnectionBuilder, LogLevel } from '@microsoft/signalr';
import { useAuth } from '../auth/authContext';
import { Match } from '../../intefaces/iMatch';
import { Message } from '../../intefaces/iMessage';
import api from '../../api/axiosInstance';

type ChatMessage = { author: 'me' | 'them'; text: string; id?: string };
type MessageMap = Record<string, ChatMessage[]>;

export function ChatPage() {
    const [matches, setMatches] = React.useState<Match[]>([]);
    const [selectedMatchId, setSelectedMatchId] = React.useState('');
    const [inputValue, setInputValue] = React.useState('');
    const [messages, setMessages] = React.useState<MessageMap>({});
    const [connection, setConnection] = React.useState<HubConnection | null>(null);
    const [loading, setLoading] = React.useState(true);
    const { token } = useAuth();
    const scrollRef = React.useRef<HTMLDivElement>(null);

    // Automatikus görgetés az aljára
    React.useEffect(() => {
        if (scrollRef.current) {
            scrollRef.current.scrollTop = scrollRef.current.scrollHeight;
        }
    }, [messages, selectedMatchId]);

    // 1. Matchek betöltése
    React.useEffect(() => {
        const fetchMatches = async () => {
            try {
                const response = await api.get("/Chat/matches", {
                    headers: { "Authorization": `Bearer ${token}` }
                });
                setMatches(response.data);
            } catch (error) {
                console.error("Hiba a matcheknél:", error);
            } finally {
                setLoading(false);
            }
        };
        if (token) fetchMatches();
    }, [token]);

    // 2. SignalR Kapcsolat felépítése
    React.useEffect(() => {
        if (!token) return;
        const newConnection = new HubConnectionBuilder()
            .withUrl("https://localhost:7160/chathub", { accessTokenFactory: () => token })
            .withAutomaticReconnect()
            .configureLogging(LogLevel.Information)
            .build();
        setConnection(newConnection);
        return () => { newConnection.stop(); };
    }, [token]);

    // 3. Közös frissítő függvény a bal oldali listához
    const updateMatchList = (partnerId: string, text: string, senderName: string) => {
        setMatches((prev) => prev.map(m => {
            if (m.id?.toLowerCase() === partnerId?.toLowerCase()) {
                return { ...m, lastMessage: text, lastMessageSenderName: senderName };
            }
            return m;
        }));
    };

    // 4. SignalR Események kezelése
    React.useEffect(() => {
        if (!connection) return;

        if (connection.state === "Disconnected") {
            connection.start().then(() => {
                // Meglévő figyelők törlése az újraregisztráció előtt
                connection.off("ReceiveMessage");
                connection.off("MessageSent");

                // ÜZENET ÉRKEZIK (Kétszereződés elleni védelemmel)
                connection.on("ReceiveMessage", (savedMessage: any) => {
                    const partnerId = savedMessage.senderId;
                    setMessages((prev) => {
                        const chat = prev[partnerId] || [];
                        if (chat.some(m => m.id === savedMessage.id)) return prev;
                        return {
                            ...prev,
                            [partnerId]: [...chat, { author: 'them', text: savedMessage.content, id: savedMessage.id }]
                        };
                    });
                    updateMatchList(partnerId, savedMessage.content, "Partner");
                });

                connection.on("MessageSent", (savedMessage: any) => {

                    setMatches((prevMatches) => {
                        return prevMatches.map((match) => {
                            if (match.id === selectedMatchId) {
                                return {
                                    ...match,
                                    lastMessage: savedMessage.content,
                                    lastMessageSenderName: "Te"
                                };
                            }
                            return match;
                        });
                    });
                });
            }).catch(console.error);
        }
    }, [connection, selectedMatchId]);

    // Üzenet küldése (Optimistic update-tel)
    const handleSend = async () => {
        const text = inputValue.trim();
        if (!text || !selectedMatchId || !connection) return;

        try {
            // Azonnal kitesszük a képernyőre
            setMessages(prev => ({
                ...prev,
                [selectedMatchId]: [...(prev[selectedMatchId] || []), { author: 'me', text: text }]
            }));
            setInputValue('');

            await connection.invoke("SendMessage", {
                receiverId: selectedMatchId,
                content: text
            });
        } catch (e) {
            console.error("Küldési hiba:", e);
        }
    };

    // Korábbi beszélgetések betöltése
    React.useEffect(() => {
        if (!selectedMatchId || !token) return;
        const fetchHistory = async () => {
            if (messages[selectedMatchId]?.length > 0) return;
            try {
                const res = await api.get<Message[]>(`/Chat/conversation/${selectedMatchId}`, {
                    headers: { "Authorization": `Bearer ${token}` }
                });
                const history: ChatMessage[] = res.data.map(msg => ({
                    author: msg.senderId === selectedMatchId ? 'them' : 'me',
                    text: msg.content,
                    id: (msg as any).id
                }));
                setMessages(prev => ({ ...prev, [selectedMatchId]: history }));
            } catch (e) { console.error(e); }
        };
        fetchHistory();
    }, [selectedMatchId, token]);

    const selectedMatch = matches.find(m => m.id === selectedMatchId);

    return (
        <Box sx={{ display: 'flex', flexDirection: { xs: 'column', md: 'row' }, gap: 2, height: 'calc(100vh - 120px)' }}>
            <Paper sx={{ width: { xs: '100%', md: 320 }, display: 'flex', flexDirection: 'column' }} elevation={3}>
                <Box sx={{ p: 2, borderBottom: 1, borderColor: 'divider' }}><Typography variant="h6">Csevegések</Typography></Box>
                <List sx={{ overflowY: 'auto', flex: 1 }}>
                    {loading ? <Typography sx={{ p: 2 }}>Betöltés...</Typography> :
                        matches.map((match) => (
                            <ListItem key={`${match.id}-${match.lastMessage}`} disablePadding>
                                <ListItemButton selected={match.id === selectedMatchId} onClick={() => setSelectedMatchId(match.id)}>
                                    <ListItemAvatar><Avatar src={`data:image/png;base64,${match.picture}`} /></ListItemAvatar>
                                    <ListItemText
                                        primary={match.fullName}
                                        secondary={match.lastMessage ? `${match.lastMessageSenderName}: ${match.lastMessage}` : "Kezdjetek el beszélni!"}
                                    />
                                </ListItemButton>
                            </ListItem>
                        ))
                    }
                </List>
            </Paper>

            <Box sx={{ flex: 1, display: 'flex', flexDirection: 'column', minHeight: 0 }}>
                <Paper sx={{ p: 2, mb: 1 }} elevation={3}><Typography variant="h6">{selectedMatch?.fullName || 'Válassz partnert'}</Typography></Paper>
                <Paper sx={{ flex: 1, display: 'flex', flexDirection: 'column', overflow: 'hidden' }} elevation={3}>
                    <Box ref={scrollRef} sx={{ flex: 1, p: 2, overflowY: 'auto', backgroundColor: '#f5f5f5' }}>
                        {(messages[selectedMatchId] || []).map((msg, idx) => (
                            <Box key={idx} sx={{ display: 'flex', justifyContent: msg.author === 'me' ? 'flex-end' : 'flex-start', mb: 1.5 }}>
                                <Box sx={{ maxWidth: '75%', px: 2, py: 1, borderRadius: 2, bgcolor: msg.author === 'me' ? 'primary.main' : 'white', color: msg.author === 'me' ? 'white' : 'text.primary', boxShadow: 1 }}>
                                    <Typography variant="body2">{msg.text}</Typography>
                                </Box>
                            </Box>
                        ))}
                    </Box>
                    <Divider />
                    <Box sx={{ p: 2, bgcolor: 'white' }}>
                        <TextField fullWidth value={inputValue} onChange={(e) => setInputValue(e.currentTarget.value)}
                            onKeyDown={(e) => e.key === 'Enter' && handleSend()}
                            InputProps={{ endAdornment: <InputAdornment position="end"><IconButton color="primary" onClick={handleSend}><SendIcon /></IconButton></InputAdornment> }}
                        />
                    </Box>
                </Paper>
            </Box>
        </Box>
    );
}