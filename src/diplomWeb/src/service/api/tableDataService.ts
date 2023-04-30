import { AxiosResponse } from 'axios';
import { $api } from '../../http/index';
import { IEmployee } from '../../models/interfaces/IEmployee';
import { ITableParams } from '../../models/interfaces/ITableParams';
import deviceHistoryResponse from '../../models/interfaces/response/deviceHistoryResponse';
import { pageDataResponse } from '../../models/interfaces/response/pageDataResponse';

export default class TableDataService<IItem, IInfoResponse> {
	private _requestAddress = '';

	constructor(request_address: string) {
		this._requestAddress = request_address;
	}

	async getAllTableData(params: ITableParams): Promise<AxiosResponse<pageDataResponse<IItem>>> {
		return $api.get(`${this._requestAddress}/`, {
			params: params,
		});
	}

	async syncTableData(): Promise<AxiosResponse<void>> {
		return $api.post(`${this._requestAddress}/sync`);
	}

	async getTableDataInfo(id: string): Promise<AxiosResponse<IInfoResponse>> {
		return $api.get(`${this._requestAddress}/${id}`);
	}

	async putTableDataInfo(id: string, data: IInfoResponse): Promise<AxiosResponse> {
		return $api.put(`${this._requestAddress}/${id}`, data);
	}

	async getTableDataPhoto(fileId: string): Promise<AxiosResponse<File>> {
		return $api.get(`files/${fileId}`, { responseType: 'blob' });
	}

	async postTableDataPhoto(file: File): Promise<AxiosResponse<{ File: string }>> {
		const formData = new FormData();
		formData.append('image', file);
		return $api.post('files/', formData, {
			headers: {
				'Content-Type': 'multipart/form-data',
			},
		});
	}

	async deleteTableData(id: string): Promise<AxiosResponse<void>> {
		return $api.delete(`${this._requestAddress}/${id}`);
	}

	async postTableDataInfo(data: IInfoResponse): Promise<AxiosResponse> {
		return $api.post(`${this._requestAddress}`, data);
	}

	async getAllEmployeeNames(): Promise<AxiosResponse<IEmployee[]>> {
		return $api.get('users/usernames');
	}

	async getDeviceHistory(id: string): Promise<AxiosResponse<deviceHistoryResponse>> {
		return $api.get(`devices/${id}/history`);
	}
}
