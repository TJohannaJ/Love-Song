import api from '../api/axiosInstance';

export const authService = {
    login: (data: any) => api.post('/Account/login', data),
    register: (data: any) => api.post('/Account/register', data),
    refresh: (data: { accessToken: string, refreshToken: string }) => 
        api.post('/Account/refresh-token', data),
};