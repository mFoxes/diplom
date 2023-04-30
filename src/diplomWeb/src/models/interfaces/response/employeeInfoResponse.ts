import { dataInfoResponse } from './dataInfoResponse';

export interface employeeInfoResponse extends dataInfoResponse {
	Name: string;
	Email: string;
	MattermostName: string;
}
