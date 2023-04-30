import { AxiosResponse } from 'axios';

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
