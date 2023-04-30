import IHistory from '../IHistory';

export default interface deviceHistoryResponse {
	Name: string;
	InventoryNumber: string;
	History: IHistory[];
}
