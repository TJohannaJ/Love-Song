import * as React from 'react';
import {
    Avatar, Box, Divider, IconButton, InputAdornment, List,
    ListItem, ListItemAvatar, ListItemButton, ListItemText,
    Paper, TextField, Typography
} from '@mui/material';
import SendIcon from '@mui/icons-material/Send';
import { HubConnection, HubConnectionBuilder, LogLevel } from '@microsoft/signalr';
import { useAuth } from '../LoginPage/authContext';
import { Match } from '../../intefaces/iMatch';
import { Message } from '../../intefaces/iMessage';
import api from '../../api/axiosInstance';

type ChatMessage = { author: 'me' | 'them'; text: string };
type MessageMap = Record<string, ChatMessage[]>;

interface SavedMessageDTO {
    senderId: string;
    receiverId: string;
    content: string;
    timestamp?: string;
}

export function ChatPage() {
    const [matches, setMatches] = React.useState<Match[]>([]);
    const [selectedMatchId, setSelectedMatchId] = React.useState('');
    const [inputValue, setInputValue] = React.useState('');
    const [messages, setMessages] = React.useState<MessageMap>({});
    const [connection, setConnection] = React.useState<HubConnection | null>(null);
    const [loading, setLoading] = React.useState(true); // Ezt pótoltuk
    const { token } = useAuth();
    const scrollRef = React.useRef<HTMLDivElement>(null);

    React.useEffect(() => {
        if (scrollRef.current) {
            scrollRef.current.scrollTop = scrollRef.current.scrollHeight;
        }
    }, [messages, selectedMatchId]);

    // 1. Matchek betöltése az API-ból
    React.useEffect(() => {
        const fetchMatches = async () => {
            try {
                const response = await api.get("/Chat/matches", {
                    headers: {
                        "Authorization": `Bearer ${token}`
                    }
                });
                setMatches(response.data);
            } catch (error) {
                console.error("Nem sikerült betölteni a matcheket:", error);
            } finally {
                setLoading(false);
            }
        };

        if (token) fetchMatches();
    }, [token]);

    // 2. SignalR kapcsolat inicializálása
    React.useEffect(() => {
        if (!token) return;

        const newConnection = new HubConnectionBuilder()
            .withUrl("https://localhost:7160/chathub", {
                accessTokenFactory: () => token
            })
            .withAutomaticReconnect()
            .configureLogging(LogLevel.Information)
            .build();

        setConnection(newConnection);

        return () => {
            newConnection.stop();
        };
    }, [token]);

    // 3. SignalR eseménykezelők indítása
    React.useEffect(() => {
        if (connection) {
            connection.start()
                .then(() => {
                    // KRITIKUS: Töröljük a régi figyelőket, mielőtt újakat adnánk hozzá
                    connection.off("ReceiveMessage");
                    connection.off("MessageSent");

                    connection.on("ReceiveMessage", (savedMessage: SavedMessageDTO) => {
                        setMessages((prev) => ({
                            ...prev,
                            [savedMessage.senderId]: [
                                ...(prev[savedMessage.senderId] || []),
                                { author: 'them', text: savedMessage.content }
                            ],
                        }));
                    });

                    connection.on("MessageSent", (savedMessage: SavedMessageDTO) => {
                        setMessages((prev) => ({
                            ...prev,
                            [savedMessage.receiverId]: [
                                ...(prev[savedMessage.receiverId] || []),
                                { author: 'me', text: savedMessage.content }
                            ],
                        }));
                    });
                });

            // Cleanup funkció: amikor a komponens megsemmisül vagy a connection változik
            return () => {
                connection.off("ReceiveMessage");
                connection.off("MessageSent");
            };
        }
    }, [connection]);

    const handleSend = async () => {
        const text = inputValue.trim();
        if (!text || !selectedMatchId) return;

        if (connection && connection.state === "Connected") {
            try {
                const newMessage: ChatMessage = { author: 'me', text: text };
                setMessages(prev => ({
                    ...prev,
                    [selectedMatchId]: [...(prev[selectedMatchId] || []), newMessage]
                }));
                setInputValue('');

                await connection.invoke("SendMessage", {
                    receiverId: selectedMatchId,
                    content: text
                });
            } catch (e) {
                console.error("Hiba küldés közben:", e);
            }
        }
    };

    React.useEffect(() => {
        const fetchHistory = async () => {
            if (!selectedMatchId || !token) return;

            if (messages[selectedMatchId] && messages[selectedMatchId].length > 0) return;

            try {
                const response = await api.get<Message[]>(`/Chat/conversation/${selectedMatchId}`, {
                    headers: { "Authorization": `Bearer ${token}` }
                });

                const history: ChatMessage[] = response.data.map(msg => ({
                    author: msg.senderId === selectedMatchId ? 'them' : 'me',
                    text: msg.content
                }));

                setMessages(prev => ({
                    ...prev,
                    [selectedMatchId]: history
                }));
            } catch (error) {
                console.error("Nem sikerült betölteni az előzményeket:", error);
            }
        };

        fetchHistory();
    }, [selectedMatchId, token]);

    // sampleMatches helyett a valódi matches-ből keressük ki
    const selectedMatch = matches.find((m) => m.id === selectedMatchId);
    const conversation = messages[selectedMatchId] || [];

    return (
        <Box sx={{ display: 'flex', flexDirection: { xs: 'column', md: 'row' }, gap: 2, height: 'calc(100vh - 120px)' }}>
            {/* Bal oldali lista */}
            <Paper sx={{ width: { xs: '100%', md: 320 }, display: 'flex', flexDirection: 'column' }} elevation={3}>
                <Box sx={{ p: 2, borderBottom: 1, borderColor: 'divider' }}>
                    <Typography variant="h6">Csevegések</Typography>
                </Box>
                <List sx={{ overflowY: 'auto', flex: 1 }}>
                    {loading ? (
                        <Typography sx={{ p: 2 }}>Betöltés...</Typography>
                    ) : matches.length === 0 ? (
                        <Typography sx={{ p: 2 }}>Még nincsenek párjaid.</Typography>
                    ) : (

                        matches.map((match) => (
                            <ListItem key={match.id} disablePadding>
                                <ListItemButton
                                    selected={match.id === selectedMatchId}
                                    onClick={() => setSelectedMatchId(match.id)}
                                >
                                    <ListItemAvatar>
                                        <Avatar src={`data:image/png;base64,${match.picture}`} />
                                    </ListItemAvatar>
                                    <ListItemText
                                        primary={match.fullName}
                                        secondary={
                                            match.lastMessage
                                                ? `${match.lastMessageSenderName}: ${match.lastMessage}`
                                                : "Kezdjetek el beszélgetni!"
                                        }
                                    />
                                </ListItemButton>
                            </ListItem>
                        ))
                    )}
                </List>
            </Paper>

            {/* Jobb oldali chat ablak */}
            <Box sx={{ flex: 1, display: 'flex', flexDirection: 'column', minHeight: 0 }}>
                <Paper sx={{ p: 2, mb: 1 }} elevation={3}>
                    <Typography variant="h6">{selectedMatch?.fullName || 'Válassz partnert'}</Typography>
                </Paper>

                <Paper sx={{ flex: 1, display: 'flex', flexDirection: 'column', overflow: 'hidden' }} elevation={3}>
                    <Box ref={scrollRef} sx={{ flex: 1, p: 2, overflowY: 'auto', backgroundColor: '#f5f5f5' }}>
                        {conversation.map((msg, idx) => (
                            <Box key={idx} sx={{ display: 'flex', justifyContent: msg.author === 'me' ? 'flex-end' : 'flex-start', mb: 1.5 }}>
                                <Box sx={{
                                    maxWidth: '75%', px: 2, py: 1, borderRadius: 2,
                                    bgcolor: msg.author === 'me' ? 'primary.main' : 'white',
                                    color: msg.author === 'me' ? 'white' : 'text.primary',
                                    boxShadow: 1
                                }}>
                                    <Typography variant="body2">{msg.text}</Typography>
                                </Box>
                            </Box>
                        ))}
                    </Box>
                    <Divider />
                    <Box sx={{ p: 2, bgcolor: 'white' }}>
                        <TextField
                            fullWidth
                            value={inputValue}
                            onChange={(e) => setInputValue(e.currentTarget.value)}
                            onKeyDown={(e) => e.key === 'Enter' && !e.shiftKey && (e.preventDefault(), handleSend())}
                            InputProps={{
                                endAdornment: (
                                    <InputAdornment position="end">
                                        <IconButton color="primary" onClick={handleSend}>
                                            <SendIcon />
                                        </IconButton>
                                    </InputAdornment>
                                ),
                            }}
                        />
                    </Box>
                </Paper>
            </Box>
        </Box>
    );
}