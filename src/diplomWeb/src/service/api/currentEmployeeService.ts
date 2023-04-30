import { AxiosResponse } from 'axios';
import { AxiosApi } from './base/axiosApi';
import { Either } from '@sweet-monads/either';
import { ICurrentEmployee } from '../../models/interfaces/ICurrentEmployee';
import overdueCountResponse from '../../models/interfaces/response/overdueCountResponse';
import overdueListResponse from '../../models/interfaces/response/overdueListResponse';

export default class CurrentEmployeeService extends AxiosApi {
	public async getCurrentEmployee(): Promise<Either<unknown, ICurrentEmployee>> {
		const req = this._get<ICurrentEmployee>({ url: 'users/current' });

		return this._doApiRequest(req);
	}

	public async getCurrentEmployeeOverdueCount(): Promise<Either<unknown, overdueCountResponse>> {
		const req = this._get<overdueCountResponse>({ url: 'bookings/overdue' });

		return this._doApiRequest(req);
	}

	public async getCurrentEmployeeOverdue(): Promise<Either<unknown, overdueListResponse>> {
		const req = this._get<overdueListResponse>({ url: 'bookings/overdue/list' });

		return this._doApiRequest(req);
	}
}
