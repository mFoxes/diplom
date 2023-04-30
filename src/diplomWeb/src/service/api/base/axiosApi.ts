import { Either, left, right } from '@sweet-monads/either';
import axios, { AxiosRequestConfig, AxiosResponse } from 'axios';
import { inject, injectable } from 'inversify';
import { API_URL } from '../../../staticData';
import { BaseApi, RequestData } from './baseApi';
import { Types } from '../../../inversify/inversify.types';
import LocalStorageService from '../../LocalStorageService';
import { jwtResponse } from '../../../models/interfaces/response/jswResponse';

@injectable()
export class AxiosApi extends BaseApi<AxiosRequestConfig> {
	@inject(Types.AuthService) private _authService!: AuthService;
	@inject(Types.LocalStorageService) private _localStorageService!: LocalStorageService;

	private $api = axios.create({
		withCredentials: true,
		baseURL: API_URL,
	});

	constructor() {
		super();

		this.$api.interceptors.request.use(
			(config) => this._interceptOnFulfilled(config),
			(e) => this._interceptOnReject(e),
		);
	}

	protected _doApiRequest = async <TBody, TError>(
		apiRequest: Promise<AxiosResponse<TBody>>,
	): Promise<Either<AxiosResponse<TError>, TBody>> => {
		try {
			const response = await apiRequest;
			return right(response.data);
		} catch (e: any) {
			console.log(e);
			return left(e.response);
		}
	};

	protected _get = <T, R = AxiosResponse<T>>(data: RequestData<AxiosRequestConfig>): Promise<R> => {
		return this.$api.get<T, R>(data.url, data.config);
	};

	protected _post = <T, R = AxiosResponse<T>>(data: RequestData<AxiosRequestConfig>): Promise<R> => {
		return this.$api.post<T, R>(data.url, data.payload, data.config);
	};

	protected _put = <T, R = AxiosResponse<T>>(data: RequestData<AxiosRequestConfig>): Promise<R> => {
		return this.$api.put<T, R>(data.url, data.payload, data.config);
	};

	protected _delete = <T, R = AxiosResponse<T>>(data: RequestData<AxiosRequestConfig>): Promise<R> => {
		return this.$api.delete<T, R>(data.url);
	};

	private _interceptOnFulfilled(config: AxiosRequestConfig): AxiosRequestConfig {
		if (config.headers) {
			config.headers.Authorization = `Bearer ${this._localStorageService.getAccessToken()}`;
		}
		return config;
	}

	private _interceptOnReject(error: any): Promise<AxiosResponse> {
		console.error(error);

		const originalRequest = error.config;

		if (error.response.status === 401 && !originalRequest._retry) {
			originalRequest._retry = true;

			return this.makeRefresh(originalRequest);
		}

		return Promise.reject(error);
	}

	private makeRefresh = async (originalRequest: any): Promise<AxiosResponse> => {
		const refreshToken = this._localStorageService.getRefreshToken();

		if (!refreshToken) {
			return this.incorrectRefreshCase();
		}

		return this.refreshRequest(refreshToken, originalRequest);
	};

	private refreshRequest = async (refreshToken: string, originalRequest: any): Promise<AxiosResponse<any, any>> => {
		const res = await this._authService.refresh(refreshToken);

		if (res.status === 200) {
			this._localStorageService.saveJwt(res.data);
			originalRequest.headers.Authorization = `Bearer ${this._localStorageService.getAccessToken()}`;
			return axios(originalRequest);
		} else {
			this._localStorageService.removeJwt();
			return this.incorrectRefreshCase(res);
		}
	};

	private incorrectRefreshCase = (res?: AxiosResponse<jwtResponse, any>): Promise<never> => {
		// TODO: history!!!
		// history.push('/login');
		return Promise.reject(res ?? 'refresh in local storage not found');
	};
}
