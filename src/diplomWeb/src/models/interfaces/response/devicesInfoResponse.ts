import { dataInfoResponse } from './dataInfoResponse';

export interface devicesInfoResponse extends dataInfoResponse {
	Name: string;
	InventoryNumber: string;
}
