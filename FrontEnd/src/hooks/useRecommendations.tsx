import { useState, useEffect } from 'react';
import { socialService } from '../services/socialService';
import { Profile } from '../intefaces/iProfile';

export const useRecommendations = () => {
    const [profiles, setProfiles] = useState<Profile[] | null>(null);
    const [loading, setLoading] = useState(true);

    const fetchRecommendations = async () => {
        setLoading(true);
        try {
            const response = await socialService.getRecommendations();
            setProfiles(response.data);
        } catch (error) {
            console.error("Hiba történt a letöltéskor:", error);
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        fetchRecommendations();
    }, []);

    const removeProfileFromList = (id: string) => {
        setProfiles((prev) => prev?.filter(p => p.id !== id) ?? []);
    };

    const handleLike = async (id: string) => {
        try {
            await socialService.likeRecommendation(id);
            removeProfileFromList(id); 
        } catch (error) {
            console.error("Hiba a like során:", error);
        }
    };

    const handleDislike = async (id: string) => {
        try {
            await socialService.dislikeRecommendation(id);
            removeProfileFromList(id); 
        } catch (error) {
            console.error("Hiba a dislike során:", error);
        }
    };

    return { 
        profiles, 
        loading, 
        handleLike, 
        handleDislike, 
        refresh: fetchRecommendations 
    };
};