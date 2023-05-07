import { PaletteMode } from '@mui/material';
import { jwtResponse } from '../models/interfaces/response/jswResponse';
import {
	ACCESS_TOKEN,
	REFRESH_TOKEN,
	EXPIRES_IN,
	TIME_TOKEN_CREATE,
	REMEMBER_ME,
	THEME_MODE,
} from '../constants/localstorageConstants';
import { injectable } from 'inversify';

@injectable()
export default class LocalStorageService {
	public saveJwt(data: jwtResponse): void {
		localStorage.setItem(ACCESS_TOKEN, data.access_token);
		localStorage.setItem(REFRESH_TOKEN, data.refresh_token);
		localStorage.setItem(EXPIRES_IN, data.expires_in.toString());
		localStorage.setItem(TIME_TOKEN_CREATE, (Date.now() / 1000).toString());
	}

	public saveRememberMe(remember: boolean): void {
		localStorage.setItem(REMEMBER_ME, remember ? 'true' : 'false');
	}

	public getExpiresIn(): number {
		const expires_in = localStorage.getItem(EXPIRES_IN);
		if (expires_in !== null) {
			return parseInt(expires_in);
		} else {
			return 0;
		}
	}

	public getTimeTokenCreateInSeconds(): number {
		const time_token_create = localStorage.getItem(TIME_TOKEN_CREATE);
		if (time_token_create !== null) {
			return parseInt(time_token_create);
		} else {
			return 0;
		}
	}

	public getAccessToken(): string | null {
		const access_token = localStorage.getItem(ACCESS_TOKEN);
		return access_token;
	}

	public getRefreshToken(): string | null {
		const refresh_token = localStorage.getItem(REFRESH_TOKEN);
		return refresh_token;
	}

	public getRememberMe(): boolean {
		const remember = localStorage.getItem(REMEMBER_ME);
		if (remember !== null) {
			return remember === 'true';
		} else {
			return false;
		}
	}

	public removeJwt(): void {
		localStorage.removeItem(ACCESS_TOKEN);
		localStorage.removeItem(REFRESH_TOKEN);
		localStorage.removeItem(EXPIRES_IN);
		localStorage.removeItem(TIME_TOKEN_CREATE);
		localStorage.removeItem(REMEMBER_ME);
	}

	public saveThemeMode(themeMode: string): void {
		localStorage.setItem(THEME_MODE, themeMode);
	}

	public getThemeMode(): PaletteMode {
		const themeMode = localStorage.getItem(THEME_MODE) as PaletteMode;

		return themeMode || 'light';
	}
}
