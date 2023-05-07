import { IDevices } from '../models/interfaces/IDevices';
import { devicesInfoResponse } from '../models/interfaces/response/devicesInfoResponse';
import TableDataStore from './base/TableDataStore';

export default class DevicesStore extends TableDataStore<IDevices, devicesInfoResponse> {
	constructor() {
		super('devices');
	}
}
