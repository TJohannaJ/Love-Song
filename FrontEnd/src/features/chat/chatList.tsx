import { List, ListItem, ListItemButton, ListItemAvatar, Avatar, ListItemText, Paper, Box, Typography, CircularProgress } from '@mui/material';
import { Match } from '../../intefaces/iMatch';

interface Props {
    matches: Match[];
    selectedId: string;
    onSelect: (id: string) => void;
    loading: boolean;
}

export const ChatList = ({ matches, selectedId, onSelect, loading }: Props) => (
    <Paper sx={{ width: { xs: '100%', md: 320 }, display: 'flex', flexDirection: 'column' }} elevation={3}>
        <Box sx={{ p: 2, borderBottom: 1, borderColor: 'divider' }}>
            <Typography variant="h6">Chats</Typography>
        </Box>
        <List sx={{ overflowY: 'auto', flex: 1 }}>
            {loading ? (
                <Box display="flex" justifyContent="center" py={5}>
                    <CircularProgress sx={{ color: '#FF007A' }} />
                </Box>
            ) : matches.length === 0 ? (
                <Box sx={{ px: 2, py: 5, textAlign: 'center' }}>
                    <Typography color="text.secondary">
                        You don't have a match yet, check the Likes page.
                    </Typography>
                </Box>
            ) : (
                matches.map((match) => {
                    const isSelected = match.id === selectedId;
                    return (
                        <ListItem key={match.id} disablePadding>
                            <ListItemButton 
                                selected={isSelected} 
                                onClick={() => onSelect(match.id)}
                                sx={{
                                    '&.Mui-selected': {
                                        backgroundColor: 'rgba(255, 0, 122, 0.08)',
                                        borderLeft: '4px solid #FF007A',
                                        '&:hover': {
                                            backgroundColor: 'rgba(255, 0, 122, 0.12)',
                                        },
                                    },
                                }}
                            >
                                <ListItemAvatar>
                                    <Avatar 
                                        src={`data:image/png;base64,${match.picture}`} 
                                        sx={{
                                            ...(isSelected && {
                                                p: '2px',
                                                background: 'linear-gradient(135deg, #FF007A 0%, #FF5E00 100%)',
                                            })
                                        }}
                                    />
                                </ListItemAvatar>
                                <ListItemText
                                    primary={match.fullName}
                                    secondary={match.lastMessage ? `${match.lastMessageSenderName}: ${match.lastMessage}` : "Start chatting!"}
                                    slotProps={{
                                        primary: {
                                            fontWeight: isSelected ? 600 : 400,
                                        }
                                    }}
                                />
                            </ListItemButton>
                        </ListItem>
                    );
                })
            )}
        </List>
    </Paper>
);