import { FormControl, FormHelperText, Select } from '@mui/material';
import { SelectHTMLAttributes } from 'react';
import { Controller, useFormContext } from 'react-hook-form';
import { errorResponse } from '../../../models/interfaces/response/errorResponse';
import { getErrorListByName } from '../../../utilities/Utilities';

export interface IFormSelect extends SelectHTMLAttributes<HTMLSelectElement> {
	inputName: string;
	serverErrorList?: errorResponse[];
	children?: JSX.Element[];
}

export const FormSelect = ({ children, serverErrorList, inputName, ...props }: IFormSelect): JSX.Element => {
	const methods = useFormContext();

	const inputErrorList =
		methods !== null && getErrorListByName(inputName, serverErrorList, methods.formState.errors[inputName]);

	return (
		<FormControl
			sx={[{ ...props.style }, { minHeight: '75px' }]}
			error={inputErrorList && inputErrorList.length > 0}
		>
			<Controller
				name={inputName}
				render={({ field: { onChange, value } }): JSX.Element => (
					<Select
						variant='standard'
						value={value || ''}
						onChange={onChange}
						MenuProps={{ PaperProps: { sx: { maxHeight: 250 } } }}
					>
						{children}
					</Select>
				)}
				control={methods.control}
			/>
			<FormHelperText sx={{ marginLeft: '0' }}>{inputErrorList}</FormHelperText>
		</FormControl>
	);
};
