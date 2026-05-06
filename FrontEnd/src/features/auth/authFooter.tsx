import { Box, Typography, Link } from '@mui/material';
import { Link as RouterLink } from 'react-router-dom';

interface AuthFooterProps {
    text: string;
    linkText: string;
    to: string;
}

export const AuthFooter = ({ text, linkText, to }: AuthFooterProps) => (
    <Box sx={{ textAlign: 'center', mb: 2 }}>
        <Typography variant="body2">
            {text}{' '}
            <Link component={RouterLink} to={to} sx={{ fontWeight: 'bold' }}>
                {linkText}
            </Link>
        </Typography>
    </Box>
);