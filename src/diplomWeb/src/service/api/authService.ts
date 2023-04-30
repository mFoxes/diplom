import { AxiosApi } from './base/axiosApi';
import { Either } from '@sweet-monads/either';
import { jwtResponse } from '../../models/interfaces/response/jswResponse';
import { CLIENT_ID } from '../../staticData';
import { injectable } from 'inversify';

const configToJwt = {
	headers: {
		'Content-Type': 'application/x-www-form-urlencoded',
	},
};

@injectable()
export default class AuthService extends AxiosApi {
	public async authRequest(params: URLSearchParams): Promise<Either<unknown, jwtResponse>> {
		const req = this._post<jwtResponse>({ url: 'connect/token', payload: params, config: configToJwt });
		return this._doApiRequest(req);
	}

	public async login(username: string, password: string): Promise<Either<unknown, jwtResponse>> {
		const params = new URLSearchParams();
		params.append(urlSearchParamsTypeConstants.grantType, 'password');
		params.append(urlSearchParamsTypeConstants.userName, username);
		params.append(urlSearchParamsTypeConstants.password, password);
		params.append(urlSearchParamsTypeConstants.clientId, CLIENT_ID);

		return this.authRequest(params);
	}

	public async refresh(refresh_token: string): Promise<Either<unknown, jwtResponse>> {
		const params = new URLSearchParams();
		params.append(urlSearchParamsTypeConstants.grantType, 'refresh_token');
		params.append(urlSearchParamsTypeConstants.refreshToken, refresh_token);
		params.append(urlSearchParamsTypeConstants.clientId, CLIENT_ID);

		return this.authRequest(params);
	}
}
