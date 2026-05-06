import api from '../api/axiosInstance';
import { Profile } from '../intefaces/iProfile';

export const socialService = {
    getLikes: (tab: 'likes' | 'likedYou' | 'dislikes') => 
        api.get<Profile[]>(`/Likes/${tab}`),

    acceptLike: (targetId: string) => 
        api.post(`/Likes/acceptLike`, { requestedUserId: targetId }),

    declineLike: (targetId: string) => 
        api.post(`/Likes/declineLike`, { requestedUserId: targetId}),

    undoLike: (targetId: string) => 
        api.post(`/Likes/undoLike`, { requestedUserId: targetId }),

    undoDislike: (targetId: string) => 
        api.post(`/Likes/undoDislike`, { requestedUserId: targetId }),

    getRecommendations: () => 
        api.get<Profile[]>('/Recommendation'),

    likeRecommendation: (targetId: string) => 
        api.post(`/Recommendation/like`, { requestedUserId: targetId}),

    dislikeRecommendation: (targetId: string) => 
        api.post(`/Recommendation/dislike`, { requestedUserId: targetId }),
};