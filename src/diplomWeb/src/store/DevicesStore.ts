import { injectable } from 'inversify';
import { IDevices } from '../models/interfaces/IDevices';
import { IDevicesInfoResponse } from '../models/interfaces/response/IDevicesInfoResponse';
import TableDataStore from './base/TableDataStore';

@injectable()
export default class DevicesStore {
	public tableDataStore = new TableDataStore<IDevices, IDevicesInfoResponse>('devices');
}
