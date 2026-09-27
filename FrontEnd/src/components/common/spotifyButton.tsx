import { Button, CircularProgress } from "@mui/material";

interface SpotifyButtonProps {
    onClick: () => void;
    loading: boolean;
}

export function SpotifyButton({ onClick, loading }: SpotifyButtonProps) {
    return (
        <Button
            variant="contained"
            fullWidth
            disabled={loading}
            onClick={onClick}
            sx={{
                backgroundColor: '#1DB954',
                color: '#000000',
                fontWeight: 'bold',
                py: 1.5,
                '&:hover': { backgroundColor: '#17a34a' },
                textTransform: 'none',
                fontSize: '1rem'
            }}
        >
            {loading ? <CircularProgress size={24} sx={{ color: '#000' }} /> : "Connect Spotify Account"}
        </Button>
    );
}