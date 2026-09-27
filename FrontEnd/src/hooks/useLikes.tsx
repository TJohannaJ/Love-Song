import { useEffect, useState } from "preact/hooks";
import { Profile } from "../intefaces/iProfile";
import { socialService } from "../services/socialService";

export type LikeTab = 'likes' | 'likedYou' | 'dislikes';

export const useLikes = () => {
    const [profiles, setProfiles] = useState<Profile[] | null>(null);
    const [loading, setLoading] = useState(true);
    const [activeTab, setActiveTab] = useState<LikeTab>('likedYou');

    useEffect(() => {
        let isCurrent = true;
        const fetchProfiles = async () => {
            setLoading(true);
            try {
                const response = await socialService.getLikes(activeTab);
                if (isCurrent) setProfiles(response.data);
            } catch (error) {
                console.error("Hiba a letöltéskor:", error);
            } finally {
                if (isCurrent) setLoading(false);
            }
        };

        fetchProfiles();
        return () => { isCurrent = false; };
    }, [activeTab]);

    const removeProfileFromList = (id: string) => {
        setProfiles((prev) => prev?.filter(p => p.id !== id) ?? []);
    };

    // --- Új interakciós logikák a Swagger alapján ---

    const handleAcceptLike = async (id: string) => {
        try {
            await socialService.acceptLike(id);
            removeProfileFromList(id);
        } catch (error) {
            console.error("Sikertelen elfogadás:", error);
        }
    };

    const handleDeclineLike = async (id: string) => {
        try {
            await socialService.declineLike(id);
            removeProfileFromList(id);
        } catch (error) {
            console.error("Sikertelen elutasítás:", error);
        }
    };

    const handleUndo = async (id: string) => {
        try {
            // Eldöntjük, melyik visszavonást kell hívni a jelenlegi tab alapján
            if (activeTab === 'likes') {
                await socialService.undoLike(id);
            } else if (activeTab === 'dislikes') {
                await socialService.undoDislike(id);
            }
            removeProfileFromList(id);
        } catch (error) {
            console.error("Sikertelen visszavonás:", error);
        }
    };

    return { 
        profiles, 
        loading, 
        activeTab, 
        setActiveTab, 
        handleAcceptLike, 
        handleDeclineLike, 
        handleUndo 
    };
};