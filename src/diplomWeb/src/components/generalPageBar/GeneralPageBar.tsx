import { Menu } from '@mui/icons-material';
import { AppBar, Box, Button, IconButton, Toolbar, Typography } from '@mui/material';
import { observer } from 'mobx-react-lite';
import { useContext } from 'react';
import { Link as RouterLink } from 'react-router-dom';
import { Context } from '../..';
import { DRAWER_HEIGHT, DRAWER_WIDTH } from '../../staticData';
import { EmployeePanel } from '../employeePanel/EmployeePanel';
import { ChangeThemeModeButton } from '../UI/icons/changeThemeModeButton/ChangeThemeModeButton';

export const GeneralPageBar = observer((): JSX.Element => {
	const { generalStore, authStore } = useContext(Context);

	return (
		<AppBar
			color='inherit'
			position='fixed'
			sx={{
				height: `${DRAWER_HEIGHT}px`,
				justifyContent: 'center',
				width: { md: `calc(100% - ${DRAWER_WIDTH}px)` },
				ml: { md: `${DRAWER_WIDTH}px` },
				boxShadow: 'none',
				borderBottom: '1px solid rgba(0, 0, 0, 0.12)',
			}}
		>
			<Toolbar>
				<IconButton
					color='inherit'
					aria-label='open drawer'
					edge='start'
					onClick={(): void => generalStore.handleDrawerToggle()}
					sx={{ mr: 2, display: { md: 'none' } }}
				>
					<Menu />
				</IconButton>
				<Typography sx={{ flexGrow: '1', fontWeight: 'bold' }} variant='h5'>
					{generalStore.pageTitle}
				</Typography>
				<Box sx={{ gap: '20px', display: 'flex', justifyContent: 'center', alignItems: 'center' }}>
					<ChangeThemeModeButton />
					{!authStore.currentEmployee ? (
						<Button to='/login' variant='contained' component={RouterLink}>
							Вход
						</Button>
					) : (
						<EmployeePanel />
					)}
				</Box>
			</Toolbar>
		</AppBar>
	);
});
