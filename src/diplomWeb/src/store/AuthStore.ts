import { inject, injectable } from 'inversify';
import { makeAutoObservable } from 'mobx';
import { toast } from 'react-toastify';
import { Types } from '../inversify/inversify.types';
import { ICurrentEmployee } from '../models/interfaces/ICurrentEmployee';
import { IErrorResponse } from '../models/interfaces/response/IErrorResponse';
import AuthService from '../service/api/authService';
import CurrentEmployeeService from '../service/api/currentEmployeeService';
import LocalStorageService from '../service/localStorageService';
import ErrorStore from './base/helpers/ErrorStore';
import ModalConfirmStore from './base/helpers/ModalConfirmStore';
import OverdueStore from './base/helpers/OverdueStore';

@injectable()
export default class AuthStore {
	@inject(Types.AuthService) private _authService!: AuthService;
	@inject(Types.LocalStorageService) private _localStorageService!: LocalStorageService;

	public readonly errorStore = new ErrorStore();
	public readonly modalConfirm = new ModalConfirmStore();
	public readonly overdueStore = new OverdueStore();

	public readonly currentEmployeeService = new CurrentEmployeeService();

	private _currentEmployee: ICurrentEmployee | undefined;
	private _employeeMenuActive = false;

	constructor() {
		makeAutoObservable(this);
	}

	public initCurrentEmployee(): void {
		if (this._localStorageService.getAccessToken() !== null) {
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
		const res = await this._authService.login(email, password);

		if (res.isRight()) {
			this._localStorageService.saveJwt(res.value);
			this._localStorageService.saveRememberMe(remember);
			// TODO: history!!!
			//history.back();
			this.errorStore.setError(undefined);
			this.getCurrentEmployee();
		} else {
			res.value.request.response.data.Errors.forEach((item: IErrorResponse): void => {
				toast.error(item.Message);
			});
			this.errorStore.setError(res.value.request.response.data.Errors);
		}
	}

	public logout(): void {
		this.setCurrentEmployee(undefined);
		this._localStorageService.removeJwt();
		// TODO: history!!!
		//history.push('/');
	}

	public getRemainingJwtLife(): number {
		const expires_in = this._localStorageService.getExpiresIn();
		const time_token_create = this._localStorageService.getTimeTokenCreateInSeconds();
		const next_time_to_refresh = time_token_create + expires_in;
		const remaining_life_time = next_time_to_refresh - this.nowInSeconds;
		return remaining_life_time - 5 * 60;
	}

	public async refreshTokenIfNeeded(): Promise<void> {
		const refresh_token = this._localStorageService.getRefreshToken();
		if (refresh_token && this.getRemainingJwtLife() < 0) {
			const res = await this._authService.refresh(refresh_token);

			if (res.isRight()) {
				this._localStorageService.saveJwt(res.value);
				this.getCurrentEmployee();
			} else {
				this._localStorageService.removeJwt();
				throw res.value.request.response?.data?.message;
			}
		} else {
			this.getCurrentEmployee();
		}
	}

	public rememberMeCheckAndRefresh(): void {
		const noRememberMeJWTLifeTime = 60 * 60;
		const currentJWTLifeTime = this.nowInSeconds - this._localStorageService.getTimeTokenCreateInSeconds();

		if (this._localStorageService.getRememberMe()) {
			this.refreshTokenIfNeeded();
		} else if (currentJWTLifeTime > noRememberMeJWTLifeTime) {
			this._localStorageService.removeJwt();
		}
	}

	get currentEmployee(): ICurrentEmployee | undefined {
		return this._currentEmployee;
	}

	public setCurrentEmployee(data: ICurrentEmployee | undefined): void {
		this._currentEmployee = data;
	}

	public async getCurrentEmployee(): Promise<void> {
		const res = await this.currentEmployeeService.getCurrentEmployee();

		if (res.isRight()) {
			this.setCurrentEmployee(res.value);
			this.getCurrentEmployeeOverdueCount();
		} else {
			res.value.request.response.data.Errors.forEach((item: IErrorResponse): void => {
				toast.error(item.Message);
			});
		}
	}

	public async getCurrentEmployeeOverdueCount(): Promise<void> {
		const res = await this.currentEmployeeService.getCurrentEmployeeOverdueCount();

		if (res.isRight()) {
			this.overdueStore.setCurrentEmployeeOverdueCount(res.value.TotalOverdueDevice);
		}
	}

	public async getCurrentEmployeeOverdue(): Promise<void> {
		const res = await this.currentEmployeeService.getCurrentEmployeeOverdue();
		if (res.isRight()) {
			this.overdueStore.setCurrentEmployeeOverdue(res.value.Bookings);
		}
	}

	get isAdmin(): boolean {
		const isAdmin = this._currentEmployee?.Role === 'admin';
		return isAdmin;
	}
}
