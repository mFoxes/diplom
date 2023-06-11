import { Either, left, right } from '@sweet-monads/either';
import axios, { AxiosError, AxiosRequestConfig, AxiosResponse } from 'axios';
import { injectable } from 'inversify';
import { urlSearchParamsTypeConstants } from '../../../constants/authConstants';
import { URL_FACTORY } from '../../../helpers/urlFactory';
import { history } from '../../../history/history';
import { HISTORY_URL } from '../../../history/historyUrl';
import { IJwtResponse } from '../../../models/interfaces/response/IJwtResponse';
import { API_URL, CLIENT_ID } from '../../../staticData';
import LocalStorageService from '../../localStorageService';
import { CONFIG_JWT } from '../authService';
import { BaseApi, RequestData } from './baseApi';

@injectable()
export class AxiosApi extends BaseApi<AxiosRequestConfig> {
	private _localStorageService = new LocalStorageService();

	protected $api = axios.create({
		withCredentials: true,
		baseURL: API_URL,
		headers: {
			'Access-Control-Allow-Origin': 'http://localhost:5050',
			'Access-Control-Allow-Methods': 'GET,PUT,POST,DELETE,PATCH,OPTIONS',
			'Access-Control-Allow-Credentials': 'include',
		},
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
	): Promise<Either<AxiosError<TError>, TBody>> => {
		this.$api.interceptors.request.use(
			(config) => this._interceptOnFulfilled(config),
			(e) => this._interceptOnReject(e),
		);
		try {
			const response = await apiRequest;
			return right(response.data);
		} catch (e: any) {
			console.log(e);
			return left(e);
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

	protected _interceptOnFulfilled(config: AxiosRequestConfig): AxiosRequestConfig {
		if (config.headers) {
			config.headers.Authorization = `Bearer ${this._localStorageService.getAccessToken()}`;
		}
		return config;
	}

	protected _interceptOnReject(error: any): Promise<AxiosResponse> {
		console.error(error);

		const originalRequest = error.config;

		if (error.response.status === 401 && !originalRequest._retry) {
			originalRequest._retry = true;

			return this.makeRefresh(originalRequest);
		}

		return Promise.reject(error);
	}

	protected makeRefresh = async (originalRequest: any): Promise<AxiosResponse> => {
		const refreshToken = this._localStorageService.getRefreshToken();

		if (!refreshToken) {
			return this.incorrectRefreshCase();
		}

		return this.refreshRequest(refreshToken, originalRequest);
	};

	protected refreshRequest = async (refreshToken: string, originalRequest: any): Promise<AxiosResponse<any, any>> => {
		const params = new URLSearchParams();
		params.append(urlSearchParamsTypeConstants.grantType, 'refresh_token');
		params.append(urlSearchParamsTypeConstants.refreshToken, refreshToken);
		params.append(urlSearchParamsTypeConstants.clientId, CLIENT_ID);

		const req = this._post<IJwtResponse>({ url: URL_FACTORY.token, payload: params, config: CONFIG_JWT });

		const res = await this._doApiRequest(req);

		if (res.isRight()) {
			this._localStorageService.saveJwt(res.value);
			originalRequest.headers.Authorization = `Bearer ${this._localStorageService.getAccessToken()}`;
			return axios(originalRequest);
		} else {
			this._localStorageService.removeJwt();
			return this.incorrectRefreshCase(res.value);
		}
	};

	protected incorrectRefreshCase = (res?: AxiosError<unknown>): Promise<never> => {
		history.push(HISTORY_URL.login);
		return Promise.reject(res ?? 'refresh in local storage not found');
	};
}
