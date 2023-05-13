import { IDataInfoResponse } from './IDataInfoResponse';

export interface IDevicesInfoResponse extends IDataInfoResponse {
	Name: string;
	InventoryNumber: string;
}
