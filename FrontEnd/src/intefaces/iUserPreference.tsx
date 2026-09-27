export interface UserPreference {
    minAge: number;
    maxAge: number;
    gender: string;
    city?: string;
    commonSongOnly: boolean;
}