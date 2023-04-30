import { AxiosResponse } from 'axios';
import { $api } from '../http';
import { jwtResponse } from '../models/interfaces/response/jswResponse';
import { CLIENT_ID } from '../staticData';

export const configToJwt = {
	headers: {
		'Content-Type': 'application/x-www-form-urlencoded',
	},
};

export default class AuthService {
	private static _params: URLSearchParams;

	static async login(username: string, password: string): Promise<AxiosResponse<jwtResponse>> {
		this.clearParams();
		this._params.append('grant_type', 'password');
		this._params.append('username', username);
		this._params.append('password', password);
		this._params.append('client_id', CLIENT_ID);

		return $api.post<jwtResponse>('connect/token', this._params, configToJwt);
	}

	static async refresh(refresh_token: string): Promise<AxiosResponse<jwtResponse>> {
		this.clearParams();
		this._params.append('grant_type', 'refresh_token');
		this._params.append('refresh_token', refresh_token);
		this._params.append('client_id', CLIENT_ID);

		return $api.post<jwtResponse>('connect/token', this._params, configToJwt);
	}

	static clearParams(): void {
		this._params = new URLSearchParams();
	}
}
