import axios, { AxiosResponse } from 'axios';
import { history } from '../../history/history';
import { IErrorType } from '../../models/interfaces/IErrorType';
import { jwtResponse } from '../../models/interfaces/response/jswResponse';
import AuthService from '../AuthService';
import LocalStorageService from '../LocalStorageService';

export default class ApiHelper {
	static handleResponseError = (error: IErrorType): Promise<AxiosResponse> => {
		const originalRequest = error.config;

		if (error.response.status === 401 && !originalRequest._retry) {
			originalRequest._retry = true;

			return this.makeRefresh(originalRequest);
		}

		return Promise.reject(error);
	};

	static makeRefresh = async (originalRequest: any): Promise<AxiosResponse> => {
		const refreshToken = LocalStorageService.getRefreshToken();

		if (!refreshToken) {
			return this.incorrectRefreshCase();
		}

		return ApiHelper.refreshRequest(refreshToken, originalRequest);
	};

	static refreshRequest = async (refreshToken: string, originalRequest: any): Promise<AxiosResponse<any, any>> => {
		const res = await AuthService.refresh(refreshToken);

		if (res.status === 200) {
			LocalStorageService.saveJwt(res.data);
			originalRequest.headers.Authorization = `Bearer ${LocalStorageService.getAccessToken()}`;
			return axios(originalRequest);
		} else {
			LocalStorageService.removeJwt();
			return this.incorrectRefreshCase(res);
		}
	};

	static incorrectRefreshCase = (res?: AxiosResponse<jwtResponse, any>): Promise<never> => {
		history.push('/login');
		return Promise.reject(res ?? 'refresh in local storage not found');
	};
}
