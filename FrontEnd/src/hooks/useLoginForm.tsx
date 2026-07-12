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
            // A backend válasza: { accessToken: "...", refreshToken: "..." }
            const { accessToken, refreshToken } = response.data;

            login(accessToken, refreshToken); // Itt az accessToken-t adjuk át
            setOpenDrawer(true);
            navigate('/', { replace: true });
        } catch (error: any) {
            alert(error.response?.data?.message || 'Hibás e-mail vagy jelszó!');
        }
    };

    return { register, handleSubmit: handleSubmit(onSubmit), errors };
};