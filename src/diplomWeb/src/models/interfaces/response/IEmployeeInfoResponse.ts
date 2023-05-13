import { IDataInfoResponse } from './IDataInfoResponse';

export interface IEmployeeInfoResponse extends IDataInfoResponse {
	Name: string;
	Email: string;
	MattermostName: string;
}
