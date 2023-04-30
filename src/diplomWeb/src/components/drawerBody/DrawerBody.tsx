import { DashboardRounded, DevicesSharp, PeopleSharp, Settings } from '@mui/icons-material';
import { AppBar, Box, Divider, List, Toolbar } from '@mui/material';
import { observer } from 'mobx-react-lite';
import React from 'react';
import { authStore } from '../..';
import { DRAWER_HEIGHT, DRAWER_WIDTH } from '../../staticData';
import { Logo } from '../logo/Logo';
import { NavigationItem } from '../navigationItem/NavigationItem';

export const DrawerBody = observer((): JSX.Element => {
	return (
		<Box>
			<AppBar
				color='inherit'
				position='fixed'
				sx={{
					width: `${DRAWER_WIDTH}px`,
					left: '0',
					height: `${DRAWER_HEIGHT}px`,
					boxShadow: 'none',
					borderBottom: '1px solid rgba(0, 0, 0, 0.12)',
					borderRight: '1px solid rgba(0, 0, 0, 0.12)',
				}}
			>
				<Box sx={{ display: 'flex', width: '100%', height: '100%', alignItems: 'center', padding: '0 24px' }}>
					<Logo />
				</Box>
			</AppBar>
			<Divider />
			<List sx={{ padding: '20px 8px' }}>
				<Toolbar
					sx={{
						height: `${DRAWER_HEIGHT}px`,
					}}
				/>
				<NavigationItem to='/' text='Дашборд' icon={<DashboardRounded />} />

				{authStore.isAdmin && (
					<>
						<NavigationItem to='/devices' text='Устройства' icon={<DevicesSharp />} />
						<NavigationItem to='/employees' text='Пользователи' icon={<PeopleSharp />} />
					</>
				)}
			</List>
		</Box>
	);
});
