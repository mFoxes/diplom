import { AxiosApi } from './base/axiosApi';
import { Either } from '@sweet-monads/either';
import { ICurrentEmployee } from '../../models/interfaces/ICurrentEmployee';
import IOverdueCountResponse from '../../models/interfaces/response/IOverdueCountResponse';
import IOverdueListResponse from '../../models/interfaces/response/IOverdueListResponse';
import { injectable } from 'inversify';
import { AxiosResponse } from 'axios';
import { IErrorResponse } from '../../models/interfaces/response/IErrorResponse';
import { URL_FACTORY } from '../../helpers/urlFactory';

@injectable()
export default class CurrentEmployeeService extends AxiosApi {
	public async getCurrentEmployee(): Promise<Either<AxiosResponse<IErrorResponse[]>, ICurrentEmployee>> {
		const req = this._get<ICurrentEmployee>({ url: URL_FACTORY.usersCurrent });

		return this._doApiRequest(req);
	}

	public async getCurrentEmployeeOverdueCount(): Promise<
		Either<AxiosResponse<IErrorResponse[]>, IOverdueCountResponse>
	> {
		const req = this._get<IOverdueCountResponse>({ url: URL_FACTORY.bookingsOverdue });

		return this._doApiRequest(req);
	}

	public async getCurrentEmployeeOverdue(): Promise<Either<AxiosResponse<IErrorResponse[]>, IOverdueListResponse>> {
		const req = this._get<IOverdueListResponse>({ url: URL_FACTORY.bookingsOverdueList });

		return this._doApiRequest(req);
	}
}
