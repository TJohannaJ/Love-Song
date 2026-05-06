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

    const handlePictureChange = (e: ChangeEvent<HTMLInputElement>) => {
        const file = e.currentTarget.files?.[0];
        if (file) {
            const reader = new FileReader();
            reader.onloadend = () => {
                const base64String = reader.result as string;
                const cleanBase64 = base64String.split(',')[1];
                setProfile(prev => prev ? { ...prev, picture: cleanBase64 } : null);
            };
            reader.readAsDataURL(file);
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