import { TextField } from '@mui/material';
import { observer } from 'mobx-react-lite';
import { ChangeEvent, InputHTMLAttributes } from 'react';
import { Path, useFormContext, UseFormRegisterReturn } from 'react-hook-form';
import { getErrorListByName } from '../../../utilities/Utilities';
import ErrorStore from '../../../store/base/helpers/ErrorStore';

export interface IInput<T> extends InputHTMLAttributes<HTMLInputElement> {
	label?: string;
	inputName: string;
	serverErrorStore?: ErrorStore;
	hasErrorField?: boolean;
}

export const Input = observer(
	<T,>({ inputName, label, serverErrorStore, hasErrorField, ...props }: IInput<T>): JSX.Element => {
		const methods = useFormContext();

		const inputErrorList =
			methods !== null &&
			getErrorListByName(inputName, serverErrorStore?.error, methods.formState.errors[inputName]);

		console.log('serverErrorStore?.error', serverErrorStore?.error);
		console.log('inputName', inputName);

		const registerData = methods !== null ? methods.register(inputName as Path<T>) : {};

		const { onChange, ...params } = registerData as UseFormRegisterReturn;

		const inputErrorListIsNotEmpty = inputErrorList && inputErrorList.length > 0;

		return (
			<TextField
				{...params}
				onChange={(e: ChangeEvent<HTMLInputElement | HTMLTextAreaElement>): void => {
					onChange(e);
					if (inputErrorListIsNotEmpty) {
						serverErrorStore?.removeErrorByName(inputName);
					}
				}}
				inputProps={{ ...props }}
				error={inputErrorListIsNotEmpty}
				label={label}
				helperText={inputErrorList}
				variant='standard'
				sx={[hasErrorField ? { minHeight: '75px' } : { width: '100%' }, { ...props.style }]}
			/>
		);
	},
);
