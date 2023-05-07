import { Brightness4, Brightness7 } from '@mui/icons-material';
import { IconButton } from '@mui/material';
import { observer } from 'mobx-react-lite';
import { useInject } from '../../../../hooks/useInject';
import { Types } from '../../../../inversify/inversify.types';
import GeneralStore from '../../../../store/GeneralStore';

export const ChangeThemeModeButton = observer((): JSX.Element => {
	const generalStore = useInject<GeneralStore>(Types.GeneralStore);

	return (
		<IconButton sx={{ ml: 1 }} onClick={(): void => generalStore.swapThemeMode()} color='inherit'>
			{generalStore.themeMode === 'dark' ? <Brightness7 /> : <Brightness4 sx={{ opacity: '0.87' }} />}
		</IconButton>
	);
});
