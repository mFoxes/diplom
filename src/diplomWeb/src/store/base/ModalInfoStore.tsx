import { makeAutoObservable } from 'mobx';
import ErrorStore from './helpers/ErrorStore';
import ModalStore from './ModalStore';
import { injectable } from 'inversify';

@injectable()
export default class ModalInfoStore<IInfoResponse> {
	private _tableDataInfoId = '';
	private _tableDataInfo: IInfoResponse | undefined;
	private _tableDataPhoto: File = new File([], '');

	public readonly errorStore = new ErrorStore();
	public readonly modalStore = new ModalStore();

	constructor() {
		makeAutoObservable(this);
	}

	get tableDataInfoId(): string {
		return this._tableDataInfoId;
	}

	public setTableDataInfoId(data: string): void {
		this._tableDataInfoId = data;
	}

	get tableDataInfo(): IInfoResponse | undefined {
		return this._tableDataInfo;
	}

	public setTableDataInfo(data: IInfoResponse | undefined): void {
		this._tableDataInfo = data;
	}

	get tableDataPhoto(): File {
		return this._tableDataPhoto;
	}

	public setTableDataPhoto(data: File): void {
		this._tableDataPhoto = data;
	}

	public resetTableDataInfo(): void {
		this.setTableDataInfo(undefined);
		this.setTableDataInfoId('');
		this.setTableDataPhoto(new File([], ''));
	}
}
