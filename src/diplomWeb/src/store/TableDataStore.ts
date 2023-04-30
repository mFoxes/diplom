import { AxiosResponse } from 'axios';
import { action, computed, makeObservable, observable } from 'mobx';
import { Path, UseFormReturn } from 'react-hook-form';
import { IData } from '../models/interfaces/IData';
import { IErrorType } from '../models/interfaces/IErrorType';
import { ITableParams } from '../models/interfaces/ITableParams';
import { dataInfoResponse } from '../models/interfaces/response/dataInfoResponse';
import TableDataService from '../service/TableDataService';
import { REQUIRED_PHOTO_ERROR } from '../staticData';
import ModalDeviceHistoryStore from './ModalDeviceHistoryStore';
import ModalConfirmStore from './ModalConfirmStore';
import ModalInfoStore from './ModalInfoStore';
import TableParamsStore from './TableParamsStore';

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
		try {
			const res = await this.service.getAllTableData(params);
			this.setItems(res.data.Items);
			this.setTotalItems(res.data.TotalItems);
			this.setTotalItemsFiltered(res.data.TotalItemsFiltered);

			if (this.paginationLength < this.params.page) {
				this.params.handleChange(this.paginationLength);
				this.updateTableData();
			}
		} catch (e: IErrorType) {
			console.log(e);
		}
	}

	@action
	public async getTableDataInfo(): Promise<void> {
		try {
			const res = await this.service.getTableDataInfo(this.modalInfo.tableDataInfoId);

			this.modalInfo.setTableDataInfo(res.data);
		} catch (e: IErrorType) {
			console.log(e);
		}
	}

	@action
	public async getTableDataPhoto(photoId: string): Promise<AxiosResponse<File> | undefined> {
		try {
			if (photoId !== null) {
				return await this.service.getTableDataPhoto(photoId);
			}
		} catch (e: IErrorType) {
			this.modalInfo.errorStore.setError(e.response.data);
		}
	}

	@action
	public async postNewTableDataPhoto(file: File): Promise<AxiosResponse<{ File: string }> | undefined> {
		try {
			const res = await this.service.postTableDataPhoto(file);

			return res;
		} catch (e: IErrorType) {
			this.modalInfo.errorStore.setError(e.response.data.Errors);
		}
	}

	@action
	public async putTableDataInfo(newData: IInfoResponse): Promise<AxiosResponse | void> {
		try {
			if (newData.Id) {
				return await this.service.putTableDataInfo(newData.Id, newData);
			}
		} catch (e: IErrorType) {
			this.modalInfo.errorStore.setError(e.response.data.Errors);
		}
	}

	@action
	public async postTableDataInfo(newData: IInfoResponse): Promise<AxiosResponse | void> {
		try {
			return await this.service.postTableDataInfo(newData);
		} catch (e: IErrorType) {
			this.modalInfo.errorStore.setError(e.response.data.Errors);
		}
	}

	@action
	public async deleteTableData(id: string): Promise<void> {
		try {
			const res = await this.service.deleteTableData(id);

			if (res.status === 200) {
				this.modalConfirm.modalStore.handleClose();

				this.updateTableData();
			}
		} catch (e: IErrorType) {
			this.modalConfirm.errorStore.setError(e.response.data.Errors);
		}
	}

	@action
	public updateTableData(): void {
		const params = this.params.getTableParams();
		this.getAllTableData(params);
	}

	@action
	public async updateTableInfoPhoto(photo: File): Promise<AxiosResponse<{ File: string }> | undefined> {
		const newPhotoId = this.postNewTableDataPhoto(photo);
		return newPhotoId;
	}

	@action
	public async updateTableInfo(originTableDataInfo: IInfoResponse, updateMethod?: () => void): Promise<void> {
		if (originTableDataInfo.Id) {
			const res = await this.putTableDataInfo(originTableDataInfo);

			if (res && res.status === 200) {
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
		const res = await this.postTableDataInfo(originTableDataInfo);

		if (res && res.status === 200) {
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
		const res = await this.updateTableInfoPhoto(file);

		if (res && res.status === 200) {
			originDataInfo.PhotoId = res.data.File;
			updateTableInfoMethod(originDataInfo);
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
		try {
			const res = await this.service.getDeviceHistory(id);
			this.modalDeviceHistory.setDeviceHistory(res.data);
		} catch (e: IErrorType) {
			console.log(e);
		}
	}
}
