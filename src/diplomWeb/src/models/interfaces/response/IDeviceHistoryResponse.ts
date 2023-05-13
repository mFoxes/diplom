import IHistory from '../IHistory';

export default interface IDeviceHistoryResponse {
	Name: string;
	InventoryNumber: string;
	History: IHistory[];
}
