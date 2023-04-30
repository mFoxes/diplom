import axios, { AxiosRequestConfig, AxiosResponse } from 'axios';
import { IErrorType } from '../models/interfaces/IErrorType';
import { jwtResponse } from '../models/interfaces/response/jswResponse';
import ApiHelper from '../service/helpers/ApiHelpers';
import LocalStorageService from '../service/LocalStorageService';
import { API_URL } from '../staticData';

export const $api = axios.create({
	withCredentials: true,
	baseURL: API_URL,
});

$api.interceptors.request.use((config: AxiosRequestConfig) => {
	if (config.headers) {
		config.headers.Authorization = `Bearer ${LocalStorageService.getAccessToken()}`;
	}
	return config;
});

$api.interceptors.response.use(
	(response: AxiosResponse<jwtResponse>): AxiosResponse<jwtResponse> => {
		return response;
	},
	(error: IErrorType): Promise<AxiosResponse<jwtResponse>> => {
		return ApiHelper.handleResponseError(error);
	},
);
