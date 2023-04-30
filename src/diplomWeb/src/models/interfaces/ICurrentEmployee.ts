import { employeeInfoResponse } from './response/employeeInfoResponse';

export interface ICurrentEmployee extends employeeInfoResponse {
	Role: 'admin' | 'employee';
}
