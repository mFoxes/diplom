import { PaletteMode } from '@mui/material';
import { jwtResponse } from '../models/interfaces/response/jswResponse';

export default class LocalStorageService {
	private readonly ACCESS_TOKEN = 'access_token';
	private readonly REFRESH_TOKEN = 'refresh_token';
	private readonly EXPIRES_IN = 'expires_in';
	private readonly TIME_TOKEN_CREATE = 'time_token_create';
	private readonly REMEMBER_ME = 'remember';

	private readonly THEME_MODE = 'theme_mode';

	public saveJwt(data: jwtResponse): void {
		localStorage.setItem(this.ACCESS_TOKEN, data.access_token);
		localStorage.setItem(this.REFRESH_TOKEN, data.refresh_token);
		localStorage.setItem(this.EXPIRES_IN, data.expires_in.toString());
		localStorage.setItem(this.TIME_TOKEN_CREATE, (Date.now() / 1000).toString());
	}

	public saveRememberMe(remember: boolean): void {
		localStorage.setItem(this.REMEMBER_ME, remember ? 'true' : 'false');
	}

	public getExpiresIn(): number {
		const expires_in = localStorage.getItem(this.EXPIRES_IN);
		if (expires_in !== null) {
			return parseInt(expires_in);
		} else {
			return 0;
		}
	}

	public getTimeTokenCreateInSeconds(): number {
		const time_token_create = localStorage.getItem(this.TIME_TOKEN_CREATE);
		if (time_token_create !== null) {
			return parseInt(time_token_create);
		} else {
			return 0;
		}
	}

	public getAccessToken(): string | null {
		const access_token = localStorage.getItem(this.ACCESS_TOKEN);
		return access_token;
	}

	public getRefreshToken(): string | null {
		const refresh_token = localStorage.getItem(this.REFRESH_TOKEN);
		return refresh_token;
	}

	public getRememberMe(): boolean {
		const remember = localStorage.getItem(this.REMEMBER_ME);
		if (remember !== null) {
			return remember === 'true';
		} else {
			return false;
		}
	}

	public removeJwt(): void {
		localStorage.removeItem(this.ACCESS_TOKEN);
		localStorage.removeItem(this.REFRESH_TOKEN);
		localStorage.removeItem(this.EXPIRES_IN);
		localStorage.removeItem(this.TIME_TOKEN_CREATE);
		localStorage.removeItem(this.REMEMBER_ME);
	}

	public saveThemeMode(themeMode: string): void {
		localStorage.setItem(this.THEME_MODE, themeMode);
	}

	public getThemeMode(): PaletteMode {
		const themeMode = localStorage.getItem(this.THEME_MODE) as PaletteMode;

		return themeMode || 'light';
	}
}
