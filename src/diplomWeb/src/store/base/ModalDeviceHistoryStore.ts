import { makeAutoObservable } from 'mobx';
import IDeviceHistoryResponse from '../../models/interfaces/response/IDeviceHistoryResponse';
import ModalStore from './ModalStore';
import { injectable } from 'inversify';

@injectable()
export default class ModalDeviceHistoryStore {
	private _deviceHistory: IDeviceHistoryResponse | undefined;

	public readonly modalStore = new ModalStore();

	constructor() {
		makeAutoObservable(this);
	}

	get deviceHistory(): IDeviceHistoryResponse | undefined {
		return this._deviceHistory;
	}

	public setDeviceHistory(data: IDeviceHistoryResponse | undefined): void {
		this._deviceHistory = data;
	}
}
