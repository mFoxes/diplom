import { makeAutoObservable } from 'mobx';

export default class ModalStore {
	private _modalActive = false;

	constructor() {
		makeAutoObservable(this);
	}

	get modalActive(): boolean {
		return this._modalActive;
	}

	public setModalActive(data: boolean): void {
		this._modalActive = data;
	}

	public handleOpen(): void {
		this.setModalActive(true);
	}

	public handleClose(): void {
		this.setModalActive(false);
	}
}
