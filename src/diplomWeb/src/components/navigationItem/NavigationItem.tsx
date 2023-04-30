import { ListItem, ListItemButton, ListItemText } from '@mui/material';
import { observer } from 'mobx-react-lite';
import { useContext } from 'react';
import { Link as RouterLink } from 'react-router-dom';
import { Context } from '../..';

interface INavigationItem {
	to: string;
	text: string;
	icon: JSX.Element;
}

export const NavigationItem = observer((props: INavigationItem): JSX.Element => {
	const { generalStore } = useContext(Context);

	return (
		<ListItem disablePadding>
			<ListItemButton
				to={props.to}
				component={RouterLink}
				sx={[{ gap: '16px' }, generalStore.pageTitle === props.text ? { color: '#24B3EF' } : {}]}
			>
				{props.icon}
				<ListItemText primary={props.text} />
			</ListItemButton>
		</ListItem>
	);
});
