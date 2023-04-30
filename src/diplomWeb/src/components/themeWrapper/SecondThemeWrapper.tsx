import { createTheme, ThemeProvider } from '@mui/material';
import { grey } from '@mui/material/colors';
import React from 'react';

interface ISecondThemeWrapper {
	children?: JSX.Element;
}

export const SecondThemeWrapper = ({ children, ...props }: ISecondThemeWrapper): JSX.Element => {
	const theme = createTheme({
		palette: {
			primary: {
				main: grey[200],
			},
			text: {
				primary: '#000',
			},
		},
	});

	return <ThemeProvider theme={theme}>{children}</ThemeProvider>;
};
