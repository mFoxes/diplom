import { observer } from 'mobx-react-lite';
import { useEffect } from 'react';
import { Route, Routes } from 'react-router-dom';
import { LoginPage } from './pages/Login/LoginPage';

import { Box } from '@mui/material';
import { ToastContainer } from 'react-toastify';
import { RouterNavigate } from './components/router/RouterNavigate';
import { DashboardPage } from './pages/Dashboard/DashboardPage';
import DevicesPage from './pages/Devices/DevicesPage';
import EmployeesPage from './pages/Employees/EmployeesPage';
import { GeneralPage } from './pages/General/GeneralPage';
import './style/app.scss';

import { injectStyle } from 'react-toastify/dist/inject-style';
import { useInject } from './hooks/useInject';
import { Types } from './inversify/inversify.types';
import AuthStore from './store/AuthStore';
import GeneralStore from './store/GeneralStore';

function App(): JSX.Element {
	const generalStore = useInject<GeneralStore>(Types.GeneralStore);
	const authStore = useInject<AuthStore>(Types.AuthStore);

	useEffect(() => {
		injectStyle();
		generalStore.initThemeMode();
		authStore.initCurrentEmployee();
		authStore.rememberMeCheckAndRefresh();
	}, []);

	return (
		<Box>
			<ToastContainer
				position='top-right'
				autoClose={5000}
				hideProgressBar={false}
				newestOnTop={false}
				closeOnClick
				rtl={false}
				pauseOnFocusLoss
				draggable
				pauseOnHover
				theme={generalStore.themeMode}
			/>
			<Routes>
				<Route path='/login' element={<LoginPage />} />
				<Route path='/' element={<GeneralPage />}>
					<Route path='/' element={<DashboardPage />} />
					<Route path='/devices' element={<RouterNavigate element={<DevicesPage />} />} />
					<Route path='/employees' element={<RouterNavigate element={<EmployeesPage />} />} />
					<Route path='/settings' element={<RouterNavigate element={<></>} />} />
				</Route>
			</Routes>
		</Box>
	);
}

export default observer(App);
