import * as React from 'react';
import { Box, Paper, Typography, Divider, TextField, InputAdornment, IconButton } from '@mui/material';
import SendIcon from '@mui/icons-material/Send';
import { useChat } from '../../hooks/useChat';
import { ChatList } from './chatList';

export function ChatPage() {    
    const { matches, selectedMatchId, setSelectedMatchId, messages, loading, sendMessage } = useChat();
    const [inputValue, setInputValue] = React.useState('');
    const scrollRef = React.useRef<HTMLDivElement>(null);

    const selectedMatch = matches.find(m => m.id === selectedMatchId);

    React.useEffect(() => {
        if (scrollRef.current) {
            scrollRef.current.scrollTop = scrollRef.current.scrollHeight;
        }
    }, [messages, selectedMatchId]);

    const handleSendAction = () => {
        if (!inputValue.trim()) return;
        sendMessage(inputValue.trim());
        setInputValue('');
    };

    return (
        <Box sx={{ display: 'flex', flexDirection: { xs: 'column', md: 'row' }, gap: 2, height: 'calc(100vh - 120px)' }}>
            <ChatList matches={matches} selectedId={selectedMatchId} onSelect={setSelectedMatchId} loading={loading} />

            <Box sx={{ flex: 1, display: 'flex', flexDirection: 'column', minHeight: 0 }}>
                <Paper sx={{ p: 2, mb: 1 }} elevation={3}>
                    <Typography variant="h6">{selectedMatch?.fullName || 'Válassz partnert'}</Typography>
                </Paper>

                <Paper sx={{ flex: 1, display: 'flex', flexDirection: 'column', overflow: 'hidden' }} elevation={3}>
                    <Box ref={scrollRef} sx={{ flex: 1, p: 2, overflowY: 'auto', backgroundColor: '#f5f5f5', display: 'flex', flexDirection: 'column' }}>
                        {(messages[selectedMatchId] || []).map((msg: any, idx: number) => (
                            <Box key={msg.id || idx} sx={{ display: 'flex', justifyContent: msg.author === 'me' ? 'flex-end' : 'flex-start', mb: 1.5 }}>
                                <Box sx={{ maxWidth: '75%', px: 2, py: 1, borderRadius: 2, bgcolor: msg.author === 'me' ? 'primary.main' : 'white', color: msg.author === 'me' ? 'white' : 'text.primary', boxShadow: 1 }}>
                                    <Typography variant="body2">{msg.text}</Typography>
                                </Box>
                            </Box>
                        ))}
                    </Box>
                    <Divider />
                    <Box sx={{ p: 2, bgcolor: 'white' }}>
                        <TextField 
                            fullWidth value={inputValue} disabled={!selectedMatchId}
                            onChange={(e) => setInputValue(e.currentTarget.value)}
                            onKeyDown={(e) => e.key === 'Enter' && handleSendAction()}
                            InputProps={{ 
                                endAdornment: (
                                    <InputAdornment position="end">
                                        <IconButton color="primary" onClick={handleSendAction} disabled={!inputValue.trim()}><SendIcon /></IconButton>
                                    </InputAdornment>
                                ) 
                            }}
                        />
                    </Box>
                </Paper>
            </Box>
        </Box>
    );
}