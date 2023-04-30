import { Close } from '@mui/icons-material';
import { IconButton, InputAdornment } from '@mui/material';
import { observer } from 'mobx-react-lite';

export interface IInputCleanButton {
	handleCleanField: () => void;
}

export const InputCleanButton = observer(({ handleCleanField, ...props }: IInputCleanButton): JSX.Element => {
	return (
		<InputAdornment position='end'>
			<IconButton onClick={handleCleanField}>
				<Close />
			</IconButton>
		</InputAdornment>
	);
});
