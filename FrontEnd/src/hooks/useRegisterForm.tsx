import { useForm } from 'react-hook-form';
import { useNavigate } from 'react-router-dom';
import { authService } from '../services/authService';

export const useRegisterForm = () => {
    const { register, handleSubmit, watch, formState: { errors } } = useForm();
    const navigate = useNavigate();

    const onSubmit = async (data: any) => {
        // A confirmPassword-öt nem küldjük a backendnek
        const { confirmPassword, ...registerData } = data;
        
        try {
            const response = await authService.register(registerData);

            if (response.status === 200) {
                alert('Sikeres regisztráció! Most már bejelentkezhetsz.');
                navigate('/login');
            }
        } catch (error: any) {
            alert(error.response?.data?.message || 'Hiba történt a regisztráció során!');
        }
    };

    return {
        register,
        handleSubmit: handleSubmit(onSubmit),
        errors,
        watch, // Szükséges a jelszó ellenőrzéshez
    };
};