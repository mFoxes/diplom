import { Box, Typography } from '@mui/material';
import { observer } from 'mobx-react-lite';
import { IInputDate, InputDate } from '../UI/input/InputDate';

export interface IModalDateField<T> {
	fieldName: string;
	inputAttribute: IInputDate<T>;
}

export const ModalDateField = observer(<T,>(props: IModalDateField<T>): JSX.Element => {
	return (
		<Box sx={[{ display: 'flex' }]}>
			<Typography sx={{ minWidth: '200px' }}>{props.fieldName}:</Typography>

			<InputDate style={{ width: '100%' }} {...props.inputAttribute} />
		</Box>
	);
});
