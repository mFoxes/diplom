import { Box, Typography } from '@mui/material';
import { FormSelect, IFormSelect } from '../UI/select/FormSelect';

export interface IModalSelectField {
	hasColumnDirection?: boolean;
	fieldName: string;
	children?: JSX.Element[];
	selectInputAttribute: IFormSelect;
}

export const ModalSelectField = (props: IModalSelectField): JSX.Element => {
	return (
		<Box sx={[{ display: 'flex' }, props.hasColumnDirection ? { flexDirection: 'column' } : {}]}>
			<Typography sx={{ minWidth: '200px' }}>{props.fieldName}:</Typography>

			<FormSelect style={{ width: '100%' }} {...props.selectInputAttribute}>
				{props.children}
			</FormSelect>
		</Box>
	);
};
