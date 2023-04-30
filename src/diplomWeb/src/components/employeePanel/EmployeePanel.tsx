import { RunningWithErrors } from '@mui/icons-material';
import { Badge, Box, Button, IconButton, Typography } from '@mui/material';
import { observer } from 'mobx-react-lite';
import { useContext, useRef } from 'react';
import { Context } from '../..';
import employeePhotoEmpty from '../../img/employeePhotoEmpty.png';
import { EmployeeMenu } from '../employeeMenu/EmployeeMenu';
import { DownloadableImage } from '../UI/img/DownloadableImage';

export const EmployeePanel = observer((): JSX.Element => {
	const { authStore, generalStore } = useContext(Context);

	const anchorEl = useRef<HTMLButtonElement>(null);

	return (
		<Box sx={{ display: 'flex', justifyContent: 'center', alignItems: 'center', gap: '25px' }}>
			<IconButton
				aria-label='cart'
				disabled={!authStore.overdueStore.currentEmployeeOverdueCount}
				onClick={(): void => {
					authStore.overdueStore.modalStore.handleOpen();
				}}
			>
				<Badge color='primary' badgeContent={authStore.overdueStore.currentEmployeeOverdueCount}>
					<RunningWithErrors />
				</Badge>
			</IconButton>
			<Button
				ref={anchorEl}
				sx={{ display: 'flex', gap: '5px', justifyContent: 'center', alignItems: 'center' }}
				onClick={(e): void => {
					authStore.handleMenuOpen();
				}}
			>
				<DownloadableImage
					photoId={authStore.currentEmployee?.PhotoId}
					emptyPhoto={employeePhotoEmpty}
					alt='currentEmployee'
					style={{ width: '42px', height: '42px', borderRadius: '4px', objectFit: 'cover' }}
				/>
				<Typography
					sx={{
						maxWidth: '125px',
						width: '100%',
						fontSize: '16px',
						lineClamp: '2',
						overflow: 'hidden',
						textOverflow: 'ellipsis',
						textTransform: 'capitalize',
						color: generalStore.themeMode === 'dark' ? 'white' : 'black',
						textAlign: 'start',
					}}
				>
					{authStore.currentEmployee?.Name}
				</Typography>
			</Button>
			<EmployeeMenu anchorEl={anchorEl} />
		</Box>
	);
});
