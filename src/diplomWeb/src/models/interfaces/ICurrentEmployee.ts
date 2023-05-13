import { IEmployeeInfoResponse } from './response/IEmployeeInfoResponse';

export interface ICurrentEmployee extends IEmployeeInfoResponse {
	Role: 'admin' | 'employee';
}
