import { useForm } from 'react-hook-form';
import { useNavigate } from 'react-router-dom';
import { authService } from '../services/authService';
import { useAuth } from '../features/auth/authContext';

export const useLoginForm = () => {
    const { register, handleSubmit, formState: { errors } } = useForm();
    const navigate = useNavigate();
    const { login, setOpenDrawer } = useAuth();

    const onSubmit = async (data: any) => {
        try {
            const response = await authService.login(data);
            const { token, refreshToken } = response.data;

            login(token, refreshToken || "");
            setOpenDrawer(true);
            navigate('/', { replace: true });
        } catch (error: any) {
            alert(error.response?.data?.message || 'Hiba történt!');
        }
    };

    return { register, handleSubmit: handleSubmit(onSubmit), errors };
};