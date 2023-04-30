import { AxiosApi } from './base/axiosApi';
import { Either } from '@sweet-monads/either';
import { jwtResponse } from '../../models/interfaces/response/jswResponse';
import { CLIENT_ID } from '../../staticData';

const configToJwt = {
	headers: {
		'Content-Type': 'application/x-www-form-urlencoded',
	},
};

export default class AuthService extends AxiosApi {
	public async authRequest(params: URLSearchParams): Promise<Either<unknown, jwtResponse>> {
		const req = this._post<jwtResponse>({ url: 'connect/token', payload: params, config: configToJwt });
		return this._doApiRequest(req);
	}

	public async login(username: string, password: string): Promise<Either<unknown, jwtResponse>> {
		const params = new URLSearchParams();
		params.append('grant_type', 'password');
		params.append('username', username);
		params.append('password', password);
		params.append('client_id', CLIENT_ID);

		return this.authRequest(params);
	}

	public async refresh(refresh_token: string): Promise<Either<unknown, jwtResponse>> {
		const params = new URLSearchParams();
		params.append('grant_type', 'refresh_token');
		params.append('refresh_token', refresh_token);
		params.append('client_id', CLIENT_ID);

		return this.authRequest(params);
	}
}
