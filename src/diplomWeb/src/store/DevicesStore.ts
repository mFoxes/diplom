import { IDevices } from '../models/interfaces/IDevices';
import { devicesInfoResponse } from '../models/interfaces/response/devicesInfoResponse';
import TableDataStore from './TableDataStore';

export default class DevicesStore extends TableDataStore<IDevices, devicesInfoResponse> {}
