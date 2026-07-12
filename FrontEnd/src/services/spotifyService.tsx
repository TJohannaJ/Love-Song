import api from "../api/axiosInstance";

export const getSpotifyLoginUrl = async (): Promise<string> => {
    const response = await api.get<{ url: string }>("/api/Spotify/login-url");
    return response.data.url;
};