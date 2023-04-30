import { ThemeProvider } from '@emotion/react';
import { createTheme, PaletteMode, ThemeOptions } from '@mui/material';
import { grey } from '@mui/material/colors';
import { observer } from 'mobx-react-lite';
import { useContext } from 'react';
import { Context } from '../..';

interface IThemeWrapper {
	children?: JSX.Element;
}

export const ThemeWrapper = observer(({ children, ...props }: IThemeWrapper): JSX.Element => {
	const { generalStore } = useContext(Context);

	const getDesignTokens = (mode: PaletteMode): ThemeOptions => ({
		palette: {
			mode,
			primary: {
				main: '#24B3EF',
				contrastText: '#fff',
			},
			...(mode === 'light'
				? {}
				: {
						divider: grey[700],
						background: {
							default: grey[900],
							paper: grey[900],
						},
				  }),
		},
	});

	const theme = createTheme(getDesignTokens(generalStore.themeMode));

	return <ThemeProvider theme={theme}>{children}</ThemeProvider>;
});
