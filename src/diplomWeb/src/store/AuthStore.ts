import { makeAutoObservable } from 'mobx';
import { toast } from 'react-toastify';
import { history } from '../history/history';
import { ICurrentEmployee } from '../models/interfaces/ICurrentEmployee';
import { IErrorType } from '../models/interfaces/IErrorType';
import { errorResponse } from '../models/interfaces/response/errorResponse';
import AuthService from '../service/AuthService';
import CurrentEmployeeService from '../service/CurrentEmployeeService';
import LocalStorageService from '../service/localStorageService';
import ErrorStore from './ErrorStore';
import ModalConfirmStore from './ModalConfirmStore';
import OverdueStore from './OverdueStore';

export default class AuthStore {
	public readonly errorStore = new ErrorStore();
	public readonly modalConfirm = new ModalConfirmStore();
	public readonly overdueStore = new OverdueStore();

	public readonly currentEmployeeService = new CurrentEmployeeService();

	private _currentEmployee: ICurrentEmployee | undefined;
	private _employeeMenuActive = false;

	constructor() {
		makeAutoObservable(this);

		if (LocalStorageService.getAccessToken() !== null) {
			this.getCurrentEmployee();
		}
	}

	get employeeMenuActive(): boolean {
		return this._employeeMenuActive;
	}

	public setEmployeeMenuActive(bool: boolean): void {
		this._employeeMenuActive = bool;
	}

	public handleMenuOpen(): void {
		this.setEmployeeMenuActive(true);
	}

	public handleMenuClose(): void {
		this.setEmployeeMenuActive(false);
	}

	get nowInSeconds(): number {
		return Date.now() / 1000;
	}

	public async login(email: string, password: string, remember: boolean): Promise<void> {
		try {
			const res = await AuthService.login(email, password);

			if (res.status === 200) {
				LocalStorageService.saveJwt(res.data);
				LocalStorageService.saveRememberMe(remember);
				// TODO: history!!!
				//history.back();
				this.errorStore.setError(undefined);
				this.getCurrentEmployee();
			}
		} catch (e: IErrorType) {
			e.response.data.Errors.forEach((item: errorResponse): void => {
				toast.error(item.Message);
			});
			this.errorStore.setError(e.response.data.Errors);
		}
	}

	public logout(): void {
		this.setCurrentEmployee(undefined);
		LocalStorageService.removeJwt();
		// TODO: history!!!
		//history.push('/');
	}

	public getRemainingJwtLife(): number {
		const expires_in = LocalStorageService.getExpiresIn();
		const time_token_create = LocalStorageService.getTimeTokenCreateInSeconds();
		const next_time_to_refresh = time_token_create + expires_in;
		const remaining_life_time = next_time_to_refresh - this.nowInSeconds;
		return remaining_life_time - 5 * 60;
	}

	public async refreshTokenIfNeeded(): Promise<void> {
		const refresh_token = LocalStorageService.getRefreshToken();
		if (refresh_token && this.getRemainingJwtLife() < 0) {
			try {
				const response = await AuthService.refresh(refresh_token);
				LocalStorageService.saveJwt(response.data);
				this.getCurrentEmployee();
			} catch (e: IErrorType) {
				LocalStorageService.removeJwt();
				throw e.response?.data?.message;
			}
		} else {
			this.getCurrentEmployee();
		}
	}

	public rememberMeCheckAndRefresh(): void {
		const noRememberMeJWTLifeTime = 60 * 60;
		const currentJWTLifeTime = this.nowInSeconds - LocalStorageService.getTimeTokenCreateInSeconds();

		if (LocalStorageService.getRememberMe()) {
			this.refreshTokenIfNeeded();
		} else if (currentJWTLifeTime > noRememberMeJWTLifeTime) {
			LocalStorageService.removeJwt();
		}
	}

	get currentEmployee(): ICurrentEmployee | undefined {
		return this._currentEmployee;
	}

	public setCurrentEmployee(data: ICurrentEmployee | undefined): void {
		this._currentEmployee = data;
	}

	public async getCurrentEmployee(): Promise<void> {
		try {
			const res = await this.currentEmployeeService.getCurrentEmployee();
			this.setCurrentEmployee(res.data);
			this.getCurrentEmployeeOverdueCount();
		} catch (e: IErrorType) {
			e.response.data.Errors.forEach((item: errorResponse): void => {
				toast.error(item.Message);
			});
			console.log(e);
		}
	}

	public async getCurrentEmployeeOverdueCount(): Promise<void> {
		try {
			const res = await this.currentEmployeeService.getCurrentEmployeeOverdueCount();
			this.overdueStore.setCurrentEmployeeOverdueCount(res.data.TotalOverdueDevice);
		} catch (e: IErrorType) {
			console.log(e);
		}
	}

	public async getCurrentEmployeeOverdue(): Promise<void> {
		try {
			const res = await this.currentEmployeeService.getCurrentEmployeeOverdue();
			this.overdueStore.setCurrentEmployeeOverdue(res.data.Bookings);
		} catch (e: IErrorType) {
			console.log(e);
		}
	}

	get isAdmin(): boolean {
		const isAdmin = this._currentEmployee?.Role === 'admin';
		return isAdmin;
	}
}
