import { Box, Typography } from '@mui/material';

export interface IModalTextField {
	fieldName: string;
	fieldText?: string;
}

export const ModalTextField = (props: IModalTextField): JSX.Element => {
	return (
		<Box sx={{ display: 'flex' }}>
			<Typography sx={{ width: '200px' }}>{props.fieldName}:</Typography>

			<Typography sx={{ wordWrap: 'break-word', width: 'calc(100% - 200px)' }}>{props.fieldText}</Typography>
		</Box>
	);
};
