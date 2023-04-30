import { IData } from './IData';

export default interface IDashboard extends IData {
	Id: string;
	DeviceId: string;
	Name: string;
	InventoryNumber: string;
	State: 'booked' | 'free';
	TakedBy: string;
	TakeAt: Date;
	ReturnAt: Date;
	UserId: string;
}
