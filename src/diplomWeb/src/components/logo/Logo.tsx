import { Box, Typography } from '@mui/material';
import grandma from '../../img/grandma.jpg';

export const Logo = (): JSX.Element => {
	return (
		<Box sx={{ display: 'flex', gap: '10px', alignItems: 'center' }}>
			<img
				src={grandma}
				alt=''
				style={{ borderRadius: '5px', width: '50px', height: '50px', objectFit: 'cover' }}
			/>
			<Typography variant='h6' sx={{ fontWeight: 'bold' }}>
				Приложение
			</Typography>
		</Box>
	);
};
