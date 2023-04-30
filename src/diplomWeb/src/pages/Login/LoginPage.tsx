import { yupResolver } from '@hookform/resolvers/yup';
import { observer } from 'mobx-react-lite';
import { useContext, useEffect } from 'react';
import { FormProvider, SubmitHandler, useForm } from 'react-hook-form';
import { Input } from '../../components/UI/input/Input';
import { ILogin } from '../../models/interfaces/ILogin';
import { LoginSchema } from '../../models/schemas/LoginSchema';

import { KeyboardBackspace } from '@mui/icons-material';
import { Box, Button, Checkbox, CssBaseline, FormControlLabel, IconButton } from '@mui/material';
import { Context } from '../..';
import { Logo } from '../../components/logo/Logo';
import { history } from '../../history/history';
import { nameof } from '../../utilities/Utilities';

export const LoginPage = observer((): JSX.Element => {
	const { authStore } = useContext(Context);

	const methods = useForm<ILogin>({
		mode: 'all',
		resolver: yupResolver(LoginSchema),
		defaultValues: {
			remember: false,
		},
	});

	const {
		register,
		reset,
		formState: { errors },
		handleSubmit,
	} = methods;

	const onSubmit: SubmitHandler<ILogin> = async (data): Promise<void> => {
		await authStore.login(data.Login, data.Password, data.remember);
	};

	const clearErrorFields = (): void => {
		authStore.errorStore.setError(undefined);
		reset();
	};

	useEffect(() => {
		authStore.errorStore.setError(undefined);
	}, [methods.watch('Login'), methods.watch('Password')]);

	useEffect(() => {
		clearErrorFields();
	}, []);

	return (
		<FormProvider {...methods}>
			<Box
				sx={{
					position: 'relative',
					display: 'flex',
					alignItems: 'center',
					justifyContent: 'center',
					minWidth: '100vw',
					minHeight: '100vh',
				}}
			>
				<CssBaseline />
				<IconButton
					onClick={(): void => {
						history.push('/');
					}}
					sx={{ position: 'absolute', top: '0', left: '0', padding: '20px' }}
				>
					<KeyboardBackspace />
				</IconButton>
				<Box
					component={'form'}
					sx={{
						display: 'flex',
						flexDirection: 'column',
						alignItems: 'center',
						justifyContent: 'center',
						gap: '20px',
						maxWidth: '320px',
					}}
					onSubmit={handleSubmit(onSubmit)}
				>
					<Box sx={{ transform: 'scale(2)', marginBottom: '16px' }}>
						<Logo />
					</Box>

					<Input
						type='text'
						inputName={nameof<ILogin>('Login')}
						label='Логин'
						style={{ display: 'flex', width: '320px' }}
						hasErrorField
					/>

					<Input
						type='password'
						inputName={nameof<ILogin>('Password')}
						label='Пароль'
						style={{ display: 'flex', width: '320px' }}
						hasErrorField
					/>

					<FormControlLabel control={<Checkbox {...register('remember')} />} label='Запомни меня' />

					<Button type='submit' disabled={errors.Login || errors.Password ? true : false} variant='contained'>
						Войти
					</Button>
				</Box>
			</Box>
		</FormProvider>
	);
});
