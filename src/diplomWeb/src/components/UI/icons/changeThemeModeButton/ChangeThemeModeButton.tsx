import { Brightness4, Brightness7 } from '@mui/icons-material';
import { IconButton } from '@mui/material';
import { observer } from 'mobx-react-lite';
import { useContext } from 'react';
import { Context } from '../../../..';

export const ChangeThemeModeButton = observer((): JSX.Element => {
	const { generalStore } = useContext(Context);

	return (
		<IconButton sx={{ ml: 1 }} onClick={(): void => generalStore.swapThemeMode()} color='inherit'>
			{generalStore.themeMode === 'dark' ? <Brightness7 /> : <Brightness4 sx={{ opacity: '0.87' }} />}
		</IconButton>
	);
});
