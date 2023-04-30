import { AxiosResponse } from 'axios';
import { $api } from '../http';
import { ICurrentEmployee } from '../models/interfaces/ICurrentEmployee';
import overdueCountResponse from '../models/interfaces/response/overdueCountResponse';
import overdueListResponse from '../models/interfaces/response/overdueListResponse';

export default class CurrentEmployeeService {
	async getCurrentEmployee(): Promise<AxiosResponse<ICurrentEmployee>> {
		return $api.get('users/current');
	}

	async getCurrentEmployeeOverdueCount(): Promise<AxiosResponse<overdueCountResponse>> {
		return $api.get('bookings/overdue');
	}

	async getCurrentEmployeeOverdue(): Promise<AxiosResponse<overdueListResponse>> {
		return $api.get('bookings/overdue/list');
	}
}
