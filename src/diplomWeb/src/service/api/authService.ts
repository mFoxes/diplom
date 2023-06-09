import { Either } from '@sweet-monads/either';
import { AxiosError } from 'axios';
import { injectable } from 'inversify';
import { urlSearchParamsTypeConstants } from '../../constants/authConstants';
import { URL_FACTORY } from '../../helpers/urlFactory';
import { IErrorResponse } from '../../models/interfaces/response/IErrorResponse';
import { IJwtResponse } from '../../models/interfaces/response/IJwtResponse';
import { CLIENT_ID } from '../../staticData';
import { AxiosApi } from './base/axiosApi';

export const CONFIG_JWT = {
	headers: {
		'Content-Type': 'application/x-www-form-urlencoded',
	},
};

@injectable()
export default class AuthService extends AxiosApi {
	public async authRequest(params: URLSearchParams): Promise<Either<AxiosError<IErrorResponse>, IJwtResponse>> {
		const req = this._post<IJwtResponse>({
			url: URL_FACTORY.token,
			payload: params,
			config: CONFIG_JWT,
		});

		return this._doApiRequest(req);
	}

	public async login(username: string, password: string): Promise<Either<AxiosError<IErrorResponse>, IJwtResponse>> {
		const params = new URLSearchParams();
		params.append(urlSearchParamsTypeConstants.grantType, 'password');
		params.append(urlSearchParamsTypeConstants.userName, username);
		params.append(urlSearchParamsTypeConstants.password, password);
		params.append(urlSearchParamsTypeConstants.clientId, CLIENT_ID);

		return this.authRequest(params);
	}

	public async refresh(refresh_token: string): Promise<Either<AxiosError<IErrorResponse>, IJwtResponse>> {
		const params = new URLSearchParams();
		params.append(urlSearchParamsTypeConstants.grantType, 'refresh_token');
		params.append(urlSearchParamsTypeConstants.refreshToken, refresh_token);
		params.append(urlSearchParamsTypeConstants.clientId, CLIENT_ID);

		return this.authRequest(params);
	}
}
