import { TextField } from '@mui/material';
import { DatePicker } from '@mui/x-date-pickers';
import { observer } from 'mobx-react-lite';
import { Controller, Path, useFormContext, UseFormRegisterReturn } from 'react-hook-form';
import { getErrorListByName } from '../../../utilities/Utilities';
import { IInput } from './Input';

export type IInputDate<T> = IInput<T>;

export const InputDate = observer(
	<T,>({ inputName, label, serverErrorStore, hasErrorField, ...props }: IInputDate<T>): JSX.Element => {
		const methods = useFormContext();

		const inputErrorList =
			methods !== null &&
			getErrorListByName(inputName, serverErrorStore?.error, methods.formState.errors[inputName]);

		const registerData = methods !== null ? methods.register(inputName as Path<T>) : {};

		const { onChange, ...params } = registerData as UseFormRegisterReturn;

		return (
			<Controller
				name={inputName}
				render={({ field }): JSX.Element => (
					<DatePicker
						{...params}
						onChange={(value: any): void => {
							field.onChange(value);
						}}
						inputFormat='dd.MM.yyyy'
						value={methods.watch(inputName)}
						renderInput={(params): JSX.Element => (
							<TextField
								{...params}
								error={inputErrorList && inputErrorList.length > 0}
								label={label}
								helperText={inputErrorList}
								variant='standard'
								sx={[hasErrorField ? { minHeight: '75px' } : { width: '100%' }, { ...props.style }]}
							/>
						)}
					/>
				)}
				control={methods.control}
			/>
		);
	},
);
