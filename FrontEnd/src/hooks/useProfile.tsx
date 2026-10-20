import { useState, useEffect } from "preact/hooks";
import { ChangeEvent } from 'preact/compat';
import { User } from "../intefaces/iUser";
import api from "../api/axiosInstance";
import { userService } from "../services/userService";

export const useProfile = () => {
    const [profile, setProfile] = useState<User | null>(null);
    const [loading, setLoading] = useState(true);
    
    useEffect(() => {
        const getProfileDatas = async () => {
            try {
                const response = await userService.getProfile();
                setProfile(response.data);
            } catch (error) {
                console.error("Hiba történt a letöltéskor:", error);
            } finally {
                setLoading(false);
            }
        };
        getProfileDatas();
    }, []);

    const handleChange = (e: ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) => {
        const { name, value } = e.currentTarget;
        setProfile(prev => prev ? { ...prev, [name]: value } : null);
    };

    const handlePictureChange = async (e: ChangeEvent<HTMLInputElement>) => {
        const file = e.currentTarget.files?.[0];
        if (!file) return;

        // 1. Biztonsági előellenőrzés a frontendben is (pl. max 5MB)
        if (file.size > 5 * 1024 * 1024) {
            alert("A kép mérete nem lehet nagyobb 5MB-nál!");
            return;
        }

        try {
            // 2. SAS feltöltési URL elkérése a backendtől
            const { data } = await userService.getUploadUrl(file.name, file.type);

            // 3. KORVETLEN FELTÖLTÉS az Azure Blob Storage-ba (PUT kérés)
            const response = await fetch(data.uploadUrl, {
                method: 'PUT',
                headers: {
                    'x-ms-blob-type': 'BlockBlob',
                    'Content-Type': file.type,
                },
                body: file // A fájl bináris tartalma
            });

            if (!response.ok) {
                throw new Error("Sikertelen feltöltés az Azure Storage-ba.");
            }

            // 4. A felületen a végleges Azure képlinket állítjuk be
            setProfile(prev => prev ? { ...prev, pictureUrl: data.blobUrl } : null);
            alert("A kép sikeresen feltöltve!");

        } catch (error) {
            console.error("Hiba a kép feltöltése során:", error);
            alert("Hiba történt a kép feltöltésekor.");
        }
    };

    const updateProfile = async () => {
        if (!profile) return;
        try {
            await userService.updateProfile(profile);
            alert("Sikeres mentés!");
        } catch (err) {
            console.error("Hiba a mentésnél", err);
            alert("Hiba történt a mentés során.");
        }
    };

    return { profile, loading, handleChange, handlePictureChange, updateProfile };
};