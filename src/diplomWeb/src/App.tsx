import { observer } from 'mobx-react-lite';
import { useContext, useEffect } from 'react';
import { Route, Routes } from 'react-router-dom';
import { Context } from '.';
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

function App(): JSX.Element {
	const { generalStore, authStore } = useContext(Context);

	useEffect(() => {
		injectStyle();
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
