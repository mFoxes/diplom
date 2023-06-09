import { makeAutoObservable } from 'mobx';
import ErrorStore from './ErrorStore';
import ModalStore from '../ModalStore';

export default class ModalConfirmStore<IInfoResponse> {
	private _item: IInfoResponse | undefined;

	public readonly modalStore = new ModalStore();
	public readonly errorStore = new ErrorStore();

	constructor() {
		makeAutoObservable(this);
	}

	get item(): IInfoResponse | undefined {
		return this._item;
	}

	public setItem(data: IInfoResponse): void {
		this._item = data;
	}
}
