import { useState } from 'react';
import { getSpotifyLoginUrl } from '../services/spotifyService';

export const useSpotifyLink = () => {
    const [isRedirecting, setIsRedirecting] = useState(false);

    const handleLink = async () => {
        setIsRedirecting(true);
        try {
            const url = await getSpotifyLoginUrl();            
            window.location.href = url;
        } catch (error) {
            console.error("Spotify link error:", error);
            setIsRedirecting(false);
            alert("Sikertelen Spotify indítás.");
        }
    };

    return { handleLink, isRedirecting };
};