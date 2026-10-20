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
        <Box sx={{ display: 'flex', flexDirection: { xs: 'column', md: 'row' }, gap: 2, height: '100%' }}>
            <ChatList matches={matches} selectedId={selectedMatchId} onSelect={setSelectedMatchId} loading={loading} />

            <Box sx={{ flex: 1, display: 'flex', flexDirection: 'column', minHeight: 0 }}>
                <Paper sx={{ p: 2, mb: 1, backgroundColor: 'background.paper' }} elevation={3}>
                    <Typography variant="h6">{selectedMatch?.fullName || 'Select someone to chat with!'}</Typography>
                </Paper>

                <Paper sx={{ flex: 1, display: 'flex', flexDirection: 'column', overflow: 'hidden' }} elevation={3}>
                    <Box ref={scrollRef} sx={{ flex: 1, p: 2, overflowY: 'auto', backgroundColor: 'background.default', display: 'flex', flexDirection: 'column' }}>
                        {(messages[selectedMatchId] || []).map((msg: any, idx: number) => {
                            const isMe = msg.author === 'me';
                            return (
                                <Box key={msg.id || idx} sx={{ display: 'flex', justifyContent: isMe ? 'flex-end' : 'flex-start', mb: 1.5 }}>
                                    <Box 
                                        sx={{ 
                                            maxWidth: '75%', 
                                            px: 2, 
                                            py: 1, 
                                            borderRadius: 3, 
                                            background: isMe ? 'linear-gradient(135deg, #FF007A 0%, #FF5E00 100%)' : 'background.paper', 
                                            color: isMe ? 'white' : 'text.primary', 
                                            boxShadow: 1 
                                        }}
                                    >
                                        <Typography variant="body2">{msg.text}</Typography>
                                    </Box>
                                </Box>
                            );
                        })}
                    </Box>
                    <Divider />
                    <Box sx={{ p: 2, bgcolor: 'background.paper' }}>
                        <TextField 
                            fullWidth value={inputValue} disabled={!selectedMatchId}
                            onChange={(e) => setInputValue(e.currentTarget.value)}
                            onKeyDown={(e) => e.key === 'Enter' && handleSendAction()}
                            InputProps={{ 
                                endAdornment: (
                                    <InputAdornment position="end">
                                        <IconButton 
                                            onClick={handleSendAction} 
                                            disabled={!inputValue.trim()}
                                            sx={{
                                                background: inputValue.trim() ? 'linear-gradient(135deg, #FF007A 0%, #FF5E00 100%)' : 'transparent',
                                                color: inputValue.trim() ? 'white !important' : 'inherit',
                                                transition: 'all 0.2s ease-in-out',
                                                '&:hover': {
                                                    background: 'linear-gradient(135deg, #E0006C 0%, #E05300 100%)',
                                                },
                                                '&.Mui-disabled': {
                                                    background: 'transparent',
                                                }
                                            }}
                                        >
                                            <SendIcon />
                                        </IconButton>
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