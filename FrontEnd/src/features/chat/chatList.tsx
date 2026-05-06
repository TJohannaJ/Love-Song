import { List, ListItem, ListItemButton, ListItemAvatar, Avatar, ListItemText, Paper, Box, Typography } from '@mui/material';
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
            <Typography variant="h6">Csevegések</Typography>
        </Box>
        <List sx={{ overflowY: 'auto', flex: 1 }}>
            {loading ? <Typography sx={{ p: 2 }}>Betöltés...</Typography> :
                matches.map((match) => (
                    <ListItem key={match.id} disablePadding>
                        <ListItemButton selected={match.id === selectedId} onClick={() => onSelect(match.id)}>
                            <ListItemAvatar>
                                <Avatar src={`data:image/png;base64,${match.picture}`} />
                            </ListItemAvatar>
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
);