import { AxiosApi } from './base/axiosApi';
import { Either } from '@sweet-monads/either';
import { ICurrentEmployee } from '../../models/interfaces/ICurrentEmployee';
import IOverdueCountResponse from '../../models/interfaces/response/IOverdueCountResponse';
import IOverdueListResponse from '../../models/interfaces/response/IOverdueListResponse';
import { injectable } from 'inversify';
import { AxiosResponse } from 'axios';
import { IErrorResponse } from '../../models/interfaces/response/IErrorResponse';

@injectable()
export default class CurrentEmployeeService extends AxiosApi {
	public async getCurrentEmployee(): Promise<Either<AxiosResponse<IErrorResponse[]>, ICurrentEmployee>> {
		const req = this._get<ICurrentEmployee>({ url: 'users/current' });

		return this._doApiRequest(req);
	}

	public async getCurrentEmployeeOverdueCount(): Promise<
		Either<AxiosResponse<IErrorResponse[]>, IOverdueCountResponse>
	> {
		const req = this._get<IOverdueCountResponse>({ url: 'bookings/overdue' });

		return this._doApiRequest(req);
	}

	public async getCurrentEmployeeOverdue(): Promise<Either<AxiosResponse<IErrorResponse[]>, IOverdueListResponse>> {
		const req = this._get<IOverdueListResponse>({ url: 'bookings/overdue/list' });

		return this._doApiRequest(req);
	}
}
