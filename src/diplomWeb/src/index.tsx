import ReactDOM from 'react-dom/client';
import App from './App';

import { unstable_HistoryRouter as HistoryRouter } from 'react-router-dom';
import { ThemeWrapper } from './components/themeWrapper/ThemeWrapper';
import { history } from './history/history';

const root = ReactDOM.createRoot(document.getElementById('root') as HTMLElement);
root.render(
	<HistoryRouter history={history}>
		<ThemeWrapper>
			<App />
		</ThemeWrapper>
	</HistoryRouter>,
);
