import { Box, CssBaseline, Drawer, Toolbar } from '@mui/material';
import { observer } from 'mobx-react-lite';
import { Outlet } from 'react-router-dom';
import { DrawerBody } from '../../components/drawerBody/DrawerBody';
import { GeneralPageBar } from '../../components/generalPageBar/GeneralPageBar';
import { useInject } from '../../hooks/useInject';
import { Types } from '../../inversify/inversify.types';
import { DRAWER_HEIGHT, DRAWER_WIDTH } from '../../staticData';
import EmployeeStore from '../../store/EmployeesStore';
import GeneralStore from '../../store/GeneralStore';
import EmployeeInfo from '../modals/employeeInfo/EmployeeInfo';
import { ExitModal } from '../modals/general/ExitModal';
import { OverdueModal } from '../modals/overdue/OverdueModal';
import AuthStore from '../../store/AuthStore';

export const GeneralPage = observer((): JSX.Element => {
	const generalStore = useInject<GeneralStore>(Types.GeneralStore);
	const employeesStore = useInject<EmployeeStore>(Types.EmployeeStore);
	const authStore = useInject<AuthStore>(Types.AuthStore);

	return (
		<Box sx={{ display: 'flex' }}>
			<CssBaseline />
			<GeneralPageBar />
			<Box
				component='nav'
				sx={{ width: { md: DRAWER_WIDTH }, flexShrink: { md: 0 } }}
				aria-label='mailbox folders'
			>
				<Drawer
					variant='temporary'
					open={generalStore.mobileOpen}
					onClose={(): void => generalStore.handleDrawerToggle()}
					ModalProps={{
						keepMounted: true,
					}}
					sx={{
						display: { xs: 'block', md: 'none' },
						'& .MuiDrawer-paper': { boxSizing: 'border-box', width: DRAWER_WIDTH },
					}}
				>
					<DrawerBody />
				</Drawer>
				<Drawer
					variant='permanent'
					sx={{
						display: { xs: 'none', md: 'block' },
						'& .MuiDrawer-paper': { boxSizing: 'border-box', width: DRAWER_WIDTH },
					}}
					open
				>
					<DrawerBody />
				</Drawer>
			</Box>
			<Box
				component='main'
				sx={{
					flexGrow: 1,
					p: 3,
					width: { md: `calc(100% - ${DRAWER_WIDTH}px)` },
					display: 'flex',
					alignItems: 'center',
					justifyContent: 'center',
					flexDirection: 'column',
					minHeight: '100vh',
				}}
			>
				<Toolbar
					sx={{
						height: `${DRAWER_HEIGHT}px`,
					}}
				/>
				<Outlet />
			</Box>
			<ExitModal />
			{employeesStore.modalInfo.modalStore.modalActive ? <EmployeeInfo /> : ''}
			{authStore.overdueStore.modalStore.modalActive ? <OverdueModal /> : ''}
		</Box>
	);
});
