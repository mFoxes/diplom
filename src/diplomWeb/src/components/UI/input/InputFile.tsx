import { Box, Button, Typography } from '@mui/material';
import React, { ChangeEvent, InputHTMLAttributes } from 'react';
import { Path, useFormContext, UseFormRegisterReturn } from 'react-hook-form';
import ErrorStore from '../../../store/ErrorStore';
import { getErrorListByName } from '../../../utilities/Utilities';
import { SecondThemeWrapper } from '../../themeWrapper/SecondThemeWrapper';

export interface IInputFile<T> extends InputHTMLAttributes<HTMLInputElement> {
	serverErrorStore?: ErrorStore;
	inputName: string;
	setNewPhoto?: React.Dispatch<React.SetStateAction<FileList | null | undefined>>;
	accept?: string;
}

export const InputFile = <T,>({
	serverErrorStore,
	inputName,
	accept,
	setNewPhoto,
	...props
}: IInputFile<T>): JSX.Element => {
	const methods = useFormContext();

	const { register } = methods;

	const inputErrorList = getErrorListByName(inputName, serverErrorStore?.error, methods.formState.errors[inputName]);

	const registerData = register !== null ? register(inputName as Path<T>) : {};

	const { onChange, ...params } = registerData as UseFormRegisterReturn<any>;

	const fileInputChange = (e: ChangeEvent<HTMLInputElement>): void => {
		setNewPhoto && setNewPhoto(e.target.files);
		onChange(e);
		if (inputErrorList?.length !== 0) {
			serverErrorStore?.removeErrorByName(inputName);
		}
	};

	return (
		<Box sx={{ height: '75px' }}>
			<label htmlFor='upload-photo'>
				<input
					{...params}
					onChange={fileInputChange}
					id='upload-photo'
					type='file'
					style={{ display: 'none' }}
					accept={accept}
				/>

				<SecondThemeWrapper>
					<Button variant='contained' component='span'>
						{props.children}
					</Button>
				</SecondThemeWrapper>
			</label>
			<Typography sx={{ margin: '20px 0', color: '#d32f2f' }}>{inputErrorList}</Typography>
		</Box>
	);
};
