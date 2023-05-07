import { action, computed, makeObservable, observable } from 'mobx';
import { Path, UseFormReturn } from 'react-hook-form';
import { IData } from '../../models/interfaces/IData';
import { ITableParams } from '../../models/interfaces/ITableParams';
import { dataInfoResponse } from '../../models/interfaces/response/dataInfoResponse';
import TableDataService from '../../service/api/tableDataService';
import { REQUIRED_PHOTO_ERROR } from '../../staticData';
import ModalDeviceHistoryStore from './ModalDeviceHistoryStore';
import ModalInfoStore from './ModalInfoStore';
import ModalConfirmStore from './helpers/ModalConfirmStore';
import TableParamsStore from './helpers/TableParamsStore';

export default class TableDataStore<IItem extends IData, IInfoResponse extends dataInfoResponse> {
	@observable protected _items: IItem[] = [];
	@observable protected _totalItems = 0;
	@observable protected _totalItemsFiltered = 0;

	@observable public readonly service: TableDataService<IItem, IInfoResponse>;
	@observable public readonly params = new TableParamsStore();
	@observable public readonly modalInfo = new ModalInfoStore<IInfoResponse>();
	@observable public readonly modalConfirm = new ModalConfirmStore<IInfoResponse>();
	@observable public readonly modalDeviceHistory = new ModalDeviceHistoryStore();

	constructor(requestAddress: string) {
		this.service = new TableDataService<IItem, IInfoResponse>(requestAddress);

		makeObservable(this);
	}

	@computed
	get items(): IItem[] {
		return this._items;
	}

	@action
	public setItems(data: IItem[]): void {
		this._items = data;
	}

	@action
	public setItemById(index: number, data: IItem): void {
		this._items[index] = data;
	}

	@computed
	get totalItems(): number {
		return this._totalItems;
	}

	@action
	public setTotalItems(data: number): void {
		this._totalItems = data;
	}

	@computed
	get totalItemsFiltered(): number {
		return this._totalItemsFiltered;
	}

	@action
	public setTotalItemsFiltered(data: number): void {
		this._totalItemsFiltered = data;
	}

	@computed
	get paginationLength(): number {
		return Math.ceil(this.totalItemsFiltered / this.params.take);
	}

	@action
	public async getAllTableData(params: ITableParams): Promise<void> {
		const res = await this.service.getAllTableData(params);

		if (res.isRight()) {
			this.setItems(res.value.Items);
			this.setTotalItems(res.value.TotalItems);
			this.setTotalItemsFiltered(res.value.TotalItemsFiltered);

			if (this.paginationLength < this.params.page) {
				this.params.handleChange(this.paginationLength);
				this.updateTableData();
			}
		}
	}

	@action
	public async getTableDataInfo(): Promise<void> {
		const res = await this.service.getTableDataInfo(this.modalInfo.tableDataInfoId);

		if (res.isRight()) {
			this.modalInfo.setTableDataInfo(res.value);
		}
	}

	@action
	public async getTableDataPhoto(photoId: string): Promise<File | undefined> {
		if (photoId !== null) {
			const res = await this.service.getTableDataPhoto(photoId);
			if (res.isRight()) {
				return res.value;
			} else if (res.isLeft()) {
				this.modalInfo.errorStore.setError(res.value.request.response.data.Errors);
			}
		}
	}

	@action
	public async postNewTableDataPhoto(file: File): Promise<{ File: string } | undefined> {
		const res = await this.service.postTableDataPhoto(file);

		if (res.isRight()) {
			return res.value;
		} else {
			this.modalInfo.errorStore.setError(res.value.request.response.data.Errors);
		}
	}

	@action
	public async deleteTableData(id: string): Promise<void> {
		const res = await this.service.deleteTableData(id);

		if (res.isRight()) {
			this.modalConfirm.modalStore.handleClose();
			this.updateTableData();
		} else {
			this.modalInfo.errorStore.setError(res.value.request.response.data.Errors);
		}
	}

	@action
	public updateTableData(): void {
		const params = this.params.getTableParams();
		this.getAllTableData(params);
	}

	@action
	public async updateTableInfoPhoto(photo: File): Promise<{ File: string } | undefined> {
		const newPhotoId = await this.postNewTableDataPhoto(photo);
		return newPhotoId;
	}

	@action
	public async updateTableInfo(originTableDataInfo: IInfoResponse, updateMethod?: () => void): Promise<void> {
		if (originTableDataInfo.Id) {
			const res = await this.service.putTableDataInfo(originTableDataInfo.Id, originTableDataInfo);

			if (res.isRight()) {
				this.modalInfo.modalStore.handleClose();

				if (updateMethod) {
					updateMethod();
				} else {
					this.updateTableData();
				}

				this.modalInfo.errorStore.resetError();
			}
		}
	}

	@action
	public async addNewTableInfo(originTableDataInfo: IInfoResponse): Promise<void> {
		const res = await this.service.postTableDataInfo(originTableDataInfo);

		if (res.isRight()) {
			this.modalInfo.modalStore.handleClose();

			this.updateTableData();

			this.modalInfo.errorStore.resetError();
		}
	}

	@action
	public async uploadPhoto(
		file: File,
		originDataInfo: IInfoResponse,
		updateTableInfoMethod: (originDataInfo: IInfoResponse) => void,
	): Promise<void> {
		const res = await this.service.postTableDataPhoto(file);

		if (res.isRight()) {
			originDataInfo.PhotoId = res.value.File;
			updateTableInfoMethod(originDataInfo);
		} else {
			this.modalInfo.errorStore.setError(res.value.request.response.data.Errors);
		}
	}

	@action
	public async changePhoto(
		methods: UseFormReturn<IInfoResponse>,
		inputName: string,
		newPhoto: FileList | undefined,
		originDataInfo: IInfoResponse,
		updateTableInfoMethod: (originDataInfo: IInfoResponse) => void,
	): Promise<void> {
		if (!(newPhoto && newPhoto.length > 0) && !originDataInfo.PhotoId) {
			methods.setError(inputName as Path<IInfoResponse>, {
				type: 'custom',
				message: REQUIRED_PHOTO_ERROR,
			});
		} else if (newPhoto && newPhoto.length > 0) {
			await this.uploadPhoto(newPhoto[0], originDataInfo, updateTableInfoMethod);
		} else if (originDataInfo.Id) {
			updateTableInfoMethod(originDataInfo);
		}
	}

	@action async getDeviceHistory(id: string): Promise<void> {
		const res = await this.service.getDeviceHistory(id);

		if (res.isRight()) {
			this.modalDeviceHistory.setDeviceHistory(res.value);
		}
	}
}
