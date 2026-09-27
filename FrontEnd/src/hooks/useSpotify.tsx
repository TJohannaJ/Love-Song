import { useState } from 'react';
import { getSpotifyLoginUrl } from '../services/spotifyService';

export const useSpotifyLink = () => {
    const [isRedirecting, setIsRedirecting] = useState(false);

    const handleLink = async () => {
        setIsRedirecting(true);
        try {
            const url = await getSpotifyLoginUrl();            
            // Felugró ablak megnyitása:
            const width = 500, height = 600;
            const left = window.screen.width / 2 - width / 2;
            const top = window.screen.height / 2 - height / 2;

            window.open(
                url, 
                'Spotify Auth', 
                `width=${width},height=${height},top=${top},left=${left}`
            );
        } catch (error) {
            alert("Sikertelen Spotify indítás.");
        } finally {
            setIsRedirecting(false);
        }
    };

    return { handleLink, isRedirecting };
};