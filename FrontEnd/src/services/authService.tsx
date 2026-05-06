import api from '../api/axiosInstance';

export const authService = {
    login: (data: any) => api.post('/Account/login', data),
    register: (data: any) => api.post('/Account/register', data),
};