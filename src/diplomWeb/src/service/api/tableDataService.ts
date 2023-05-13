import { Either } from '@sweet-monads/either';
import { IEmployee } from '../../models/interfaces/IEmployee';
import { ITableParams } from '../../models/interfaces/ITableParams';
import IDeviceHistoryResponse from '../../models/interfaces/response/IDeviceHistoryResponse';
import { IPageDataResponse } from '../../models/interfaces/response/IPageDataResponse';
import { AxiosApi } from './base/axiosApi';
import { AxiosResponse } from 'axios';
import { IErrorResponse } from '../../models/interfaces/response/IErrorResponse';
import { URL_FACTORY } from '../../helpers/urlFactory';

export default class TableDataService<IItem, IInfoResponse> extends AxiosApi {
	private _requestAddress = '';

	constructor(request_address: string) {
		super();
		this._requestAddress = request_address;
	}

	public async getAllTableData(
		params: ITableParams,
	): Promise<Either<AxiosResponse<IErrorResponse[]>, IPageDataResponse<IItem>>> {
		const req = this._get<IPageDataResponse<IItem>>({
			url: `${this._requestAddress}/`,
			payload: {
				params: params,
			},
		});

		return this._doApiRequest(req);
	}

	public async syncTableData(): Promise<Either<AxiosResponse<IErrorResponse[]>, void>> {
		const req = this._post<void>({ url: URL_FACTORY.sync(this._requestAddress) });

		return this._doApiRequest(req);
	}

	public async getTableDataInfo(id: string): Promise<Either<AxiosResponse<IErrorResponse[]>, IInfoResponse>> {
		const req = this._get<IInfoResponse>({ url: URL_FACTORY.tableDataInfo(this._requestAddress, id) });

		return this._doApiRequest(req);
	}

	public async putTableDataInfo(
		id: string,
		data: IInfoResponse,
	): Promise<Either<AxiosResponse<IErrorResponse[]>, IInfoResponse>> {
		const req = this._put<IInfoResponse>({
			url: URL_FACTORY.tableDataInfo(this._requestAddress, id),
			payload: data,
		});

		return this._doApiRequest(req);
	}

	public async getTableDataPhoto(fileId: string): Promise<Either<AxiosResponse<IErrorResponse[]>, File>> {
		const req = this._get<File>({ url: URL_FACTORY.tableDataPhoto(fileId), payload: { responseType: 'blob' } });

		return this._doApiRequest(req);
	}

	public async postTableDataPhoto(file: File): Promise<Either<AxiosResponse<IErrorResponse[]>, { File: string }>> {
		const formData = new FormData();
		formData.append('image', file);
		const req = this._post<{ File: string }>({
			url: URL_FACTORY.files,
			payload: formData,
			config: {
				headers: {
					'Content-Type': 'multipart/form-data',
				},
			},
		});

		return this._doApiRequest(req);
	}

	public async deleteTableData(id: string): Promise<Either<AxiosResponse<IErrorResponse[]>, void>> {
		const req = this._delete<void>({ url: URL_FACTORY.tableDataInfo(this._requestAddress, id) });

		return this._doApiRequest(req);
	}

	public async postTableDataInfo(
		data: IInfoResponse,
	): Promise<Either<AxiosResponse<IErrorResponse[]>, IInfoResponse>> {
		const req = this._post<IInfoResponse>({ url: `${this._requestAddress}`, payload: data });

		return this._doApiRequest(req);
	}

	public async getAllEmployeeNames(): Promise<Either<AxiosResponse<IErrorResponse[]>, IEmployee[]>> {
		const req = this._get<IEmployee[]>({ url: URL_FACTORY.usernames });

		return this._doApiRequest(req);
	}

	public async getDeviceHistory(
		id: string,
	): Promise<Either<AxiosResponse<IErrorResponse[]>, IDeviceHistoryResponse>> {
		const req = this._get<IDeviceHistoryResponse>({ url: URL_FACTORY.deviceHistory(id) });

		return this._doApiRequest(req);
	}
}
