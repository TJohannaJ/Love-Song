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

type ChatMessage = { author: 'me' | 'them'; text: string };
type MessageMap = Record<string, ChatMessage[]>;

interface SavedMessageDTO {
    senderId: string;
    receiverId: string;
    content: string;
    timestamp?: string;
}

export function ChatPage() {
    const [matches, setMatches] = React.useState<Profile[]>([]);
    const [selectedMatchId, setSelectedMatchId] = React.useState('');
    const [inputValue, setInputValue] = React.useState('');
    const [messages, setMessages] = React.useState<MessageMap>({});
    const [connection, setConnection] = React.useState<HubConnection | null>(null);
    const [loading, setLoading] = React.useState(true); // Ezt pótoltuk
    const { token } = useAuth();

    // 1. Matchek betöltése az API-ból
    React.useEffect(() => {
        const fetchMatches = async () => {
            try {
                const response = await fetch("/Chat/matches", {
                    headers: {
                        "Authorization": `Bearer ${token}`
                    }
                });
                if (response.ok) {
                    const data = await response.json();
                    setMatches(data);
                    if (data.length > 0) setSelectedMatchId(data[0].id);
                }
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
                    console.log('Connected to ChatHub');

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

                    connection.on("Error", (errorMessage: string) => {
                        alert("Szerver hiba: " + errorMessage);
                    });
                })
                .catch(e => console.error('Connection failed: ', e));
            
            return () => {
                connection.off("ReceiveMessage");
                connection.off("MessageSent");
                connection.off("Error");
            };
        }
    }, [connection]);

    const handleSend = async () => {
        const text = inputValue.trim();
        if (!text || !selectedMatchId) return;

        if (connection && connection.state === "Connected") {
            try {
                await connection.invoke("SendMessage", {
                    receiverId: selectedMatchId,
                    content: text
                });
                setInputValue('');
            } catch (e) {
                console.error("Hiba küldés közben:", e);
            }
        }
    };

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
                                        <Avatar>{match.fullName[0]}</Avatar>
                                    </ListItemAvatar>
                                    <ListItemText
                                        primary={match.fullName}
                                        secondary={match.lastMessage || "Kezdjetek el beszélgetni!"}
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
                    <Box sx={{ flex: 1, p: 2, overflowY: 'auto', backgroundColor: '#f5f5f5' }}>
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