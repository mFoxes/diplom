import { AxiosApi } from './base/axiosApi';
import { Either } from '@sweet-monads/either';
import { ICurrentEmployee } from '../../models/interfaces/ICurrentEmployee';
import overdueCountResponse from '../../models/interfaces/response/overdueCountResponse';
import overdueListResponse from '../../models/interfaces/response/overdueListResponse';
import { injectable } from 'inversify';
import { AxiosResponse } from 'axios';
import { errorResponse } from '../../models/interfaces/response/errorResponse';

@injectable()
export default class CurrentEmployeeService extends AxiosApi {
	public async getCurrentEmployee(): Promise<Either<AxiosResponse<errorResponse[]>, ICurrentEmployee>> {
		const req = this._get<ICurrentEmployee>({ url: 'users/current' });

		return this._doApiRequest(req);
	}

	public async getCurrentEmployeeOverdueCount(): Promise<
		Either<AxiosResponse<errorResponse[]>, overdueCountResponse>
	> {
		const req = this._get<overdueCountResponse>({ url: 'bookings/overdue' });

		return this._doApiRequest(req);
	}

	public async getCurrentEmployeeOverdue(): Promise<Either<AxiosResponse<errorResponse[]>, overdueListResponse>> {
		const req = this._get<overdueListResponse>({ url: 'bookings/overdue/list' });

		return this._doApiRequest(req);
	}
}
