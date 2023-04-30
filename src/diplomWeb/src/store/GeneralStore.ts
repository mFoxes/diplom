import { PaletteMode } from '@mui/material';
import { makeAutoObservable } from 'mobx';
import LocalStorageService from '../service/LocalStorageService';

export default class GeneralStore {
	private _pageTitle = '';

	private _mobileOpen = false;

	private _themeMode: PaletteMode = 'light';

	constructor() {
		const themeMode = LocalStorageService.getThemeMode();
		this.setThemeMode(themeMode);
		makeAutoObservable(this);
	}

	get pageTitle(): string {
		return this._pageTitle;
	}

	public setPageTitle(data: string): void {
		this._pageTitle = data;
	}

	get mobileOpen(): boolean {
		return this._mobileOpen;
	}

	public setMobileOpen(data: boolean): void {
		this._mobileOpen = data;
	}

	public handleDrawerToggle(): void {
		this.setMobileOpen(!this._mobileOpen);
	}

	get themeMode(): PaletteMode {
		return this._themeMode;
	}

	public setThemeMode(data: PaletteMode): void {
		this._themeMode = data;
		LocalStorageService.saveThemeMode(data);
	}

	public swapThemeMode(): void {
		if (this._themeMode === 'light') {
			this.setThemeMode('dark');
		} else {
			this.setThemeMode('light');
		}
	}
}
