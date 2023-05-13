import { inject, injectable } from 'inversify';
import { IDevices } from '../models/interfaces/IDevices';
import { IDevicesInfoResponse } from '../models/interfaces/response/IDevicesInfoResponse';
import TableDataStore from './base/TableDataStore';
import { Types } from '../inversify/inversify.types';

@injectable()
export default class DevicesStore extends TableDataStore<IDevices, IDevicesInfoResponse> {
	// eslint-disable-next-line @typescript-eslint/no-useless-constructor
	constructor(@inject(Types.DevicesRequestAddress) requestAddress: string) {
		super(requestAddress);
	}
}
