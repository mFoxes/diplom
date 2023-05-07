import { makeAutoObservable } from 'mobx';
import deviceHistoryResponse from '../../models/interfaces/response/deviceHistoryResponse';
import ModalStore from './ModalStore';

export default class ModalDeviceHistoryStore {
	private _deviceHistory: deviceHistoryResponse | undefined;

	public readonly modalStore = new ModalStore();

	constructor() {
		makeAutoObservable(this);
	}

	get deviceHistory(): deviceHistoryResponse | undefined {
		return this._deviceHistory;
	}

	public setDeviceHistory(data: deviceHistoryResponse | undefined): void {
		this._deviceHistory = data;
	}
}
