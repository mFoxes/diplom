import { Menu } from '@mui/icons-material';
import { AppBar, Box, Button, IconButton, Toolbar, Typography } from '@mui/material';
import { observer } from 'mobx-react-lite';
import { Link as RouterLink } from 'react-router-dom';
import { useInject } from '../../hooks/useInject';
import { Types } from '../../inversify/inversify.types';
import { DRAWER_HEIGHT, DRAWER_WIDTH } from '../../staticData';
import AuthStore from '../../store/AuthStore';
import GeneralStore from '../../store/GeneralStore';
import { ChangeThemeModeButton } from '../UI/icons/changeThemeModeButton/ChangeThemeModeButton';
import { EmployeePanel } from '../employeePanel/EmployeePanel';

export const GeneralPageBar = observer((): JSX.Element => {
	const authStore = useInject<AuthStore>(Types.AuthStore);
	const generalStore = useInject<GeneralStore>(Types.GeneralStore);

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
