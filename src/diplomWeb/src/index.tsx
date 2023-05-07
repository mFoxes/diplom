import { createContext } from 'react';
import ReactDOM from 'react-dom/client';
import App from './App';
import AuthStore from './store/AuthStore';

import { unstable_HistoryRouter as HistoryRouter } from 'react-router-dom';
import { ThemeWrapper } from './components/themeWrapper/ThemeWrapper';
import { history } from './history/history';
import DashboardStore from './store/DashboardStore';
import DevicesStore from './store/DevicesStore';
import EmployeeStore from './store/EmployeesStore';
import GeneralStore from './store/GeneralStore';
import PhotosCacheStore from './store/base/helpers/PhotosCacheStore';
import SignalR from './signalR/SignalR';

interface State {
	generalStore: GeneralStore;
	authStore: AuthStore;
	photosCacheStore: PhotosCacheStore;
	employeesStore: EmployeeStore;
	devicesStore: DevicesStore;
	dashboardStore: DashboardStore;
	signalR: SignalR;
}

export const dashboardStore = new DashboardStore();
export const generalStore = new GeneralStore();
export const authStore = new AuthStore();
export const photosCacheStore = new PhotosCacheStore();
export const employeesStore = new EmployeeStore();
export const devicesStore = new DevicesStore();

export const signalR = new SignalR();

export const Context = createContext<State>({
	generalStore,
	authStore,
	photosCacheStore,
	employeesStore,
	devicesStore,
	dashboardStore,
	signalR,
});

const root = ReactDOM.createRoot(document.getElementById('root') as HTMLElement);
root.render(
	<Context.Provider
		value={{
			generalStore,
			authStore,
			photosCacheStore,
			employeesStore,
			devicesStore,
			dashboardStore,
			signalR,
		}}
	>
		<HistoryRouter history={history}>
			<ThemeWrapper>
				<App />
			</ThemeWrapper>
		</HistoryRouter>
	</Context.Provider>,
);
