// src/services/userService.ts
import api from '../api/axiosInstance';
import { User } from '../intefaces/iUser';

export interface GenerateBioRequest {
    keywords: string[];
    tone: string;
    extraContext?: string;
}

export interface GeneratedBioResponse {
    shortIntro: string;
    longIntro: string;
}

export const userService = {
    getProfile: () => api.get<User>('/User'),
    updateProfile: (data: User) => api.patch('/User/updateProfile', data),

    // ÚJ: SAS URL elkérése a C# backendtől
    getUploadUrl: (fileName: string, contentType: string) =>
        api.get<{ uploadUrl: string, blobUrl: string }>(`/User/get-upload-url?fileName=${fileName}&contentType=${contentType}`),

    generateBio: (data: GenerateBioRequest) => api.post<GeneratedBioResponse>('/User/generateBio', data),
};