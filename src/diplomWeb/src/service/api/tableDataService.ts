import { Either } from '@sweet-monads/either';
import { IEmployee } from '../../models/interfaces/IEmployee';
import { ITableParams } from '../../models/interfaces/ITableParams';
import deviceHistoryResponse from '../../models/interfaces/response/deviceHistoryResponse';
import { pageDataResponse } from '../../models/interfaces/response/pageDataResponse';
import { AxiosApi } from './base/axiosApi';

export default class TableDataService<IItem, IInfoResponse> extends AxiosApi {
	private _requestAddress = '';

	constructor(request_address: string) {
		super();
		this._requestAddress = request_address;
	}

	public async getAllTableData(params: ITableParams): Promise<Either<unknown, pageDataResponse<IItem>>> {
		const req = this._get<pageDataResponse<IItem>>({
			url: `${this._requestAddress}/`,
			payload: {
				params: params,
			},
		});

		return this._doApiRequest(req);
	}

	public async syncTableData(): Promise<Either<unknown, void>> {
		const req = this._post<void>({ url: `${this._requestAddress}/sync` });

		return this._doApiRequest(req);
	}

	public async getTableDataInfo(id: string): Promise<Either<unknown, unknown>> {
		const req = this._get<unknown>({ url: `${this._requestAddress}/${id}` });

		return this._doApiRequest(req);
	}

	public async putTableDataInfo(id: string, data: IInfoResponse): Promise<Either<unknown, unknown>> {
		const req = this._put<unknown>({ url: `${this._requestAddress}/${id}`, payload: data });

		return this._doApiRequest(req);
	}

	public async getTableDataPhoto(fileId: string): Promise<Either<unknown, File>> {
		const req = this._get<File>({ url: `files/${fileId}`, payload: { responseType: 'blob' } });

		return this._doApiRequest(req);
	}

	public async postTableDataPhoto(file: File): Promise<Either<unknown, { File: string }>> {
		const formData = new FormData();
		formData.append('image', file);
		const req = this._post<{ File: string }>({
			url: 'files/',
			payload: formData,
			config: {
				headers: {
					'Content-Type': 'multipart/form-data',
				},
			},
		});

		return this._doApiRequest(req);
	}

	public async deleteTableData(id: string): Promise<Either<unknown, void>> {
		const req = this._delete<void>({ url: `${this._requestAddress}/${id}` });

		return this._doApiRequest(req);
	}

	public async postTableDataInfo(data: IInfoResponse): Promise<Either<unknown, IInfoResponse>> {
		const req = this._post<IInfoResponse>({ url: `${this._requestAddress}`, payload: data });

		return this._doApiRequest(req);
	}

	public async getAllEmployeeNames(): Promise<Either<unknown, IEmployee[]>> {
		const req = this._get<IEmployee[]>({ url: 'users/usernames' });

		return this._doApiRequest(req);
	}

	public async getDeviceHistory(id: string): Promise<Either<unknown, deviceHistoryResponse>> {
		const req = this._get<deviceHistoryResponse>({ url: `devices/${id}/history` });

		return this._doApiRequest(req);
	}
}
