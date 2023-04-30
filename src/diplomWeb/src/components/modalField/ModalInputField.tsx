import { Box, Typography } from '@mui/material';
import { observer } from 'mobx-react-lite';
import { IInput } from '../../models/interfaces/IInput';
import { Input } from '../UI/input/Input';

export interface IModalInputField<T> {
	hasColumnDirection?: boolean;
	fieldName: string;
	inputAttribute: IInput<T>;
}

export const ModalInputField = observer(<T,>(props: IModalInputField<T>): JSX.Element => {
	return (
		<Box sx={[{ display: 'flex' }, props.hasColumnDirection ? { flexDirection: 'column' } : {}]}>
			<Typography sx={{ minWidth: '200px' }}>{props.fieldName}:</Typography>

			<Input style={{ width: '100%' }} {...props.inputAttribute} />
		</Box>
	);
});
