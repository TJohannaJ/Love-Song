import api from '../api/axiosInstance';
import { User } from '../intefaces/iUser';

export const userService = {
    getProfile: () => api.get<User>('/User'),
    updateProfile: (data: User) => api.patch('/User/updateProfile', data),
};