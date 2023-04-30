import { injectable } from 'inversify';

export interface RequestData<ConfigType, PayloadType = unknown> {
	url: string;
	payload?: PayloadType;
	config?: ConfigType;
}

export interface ApiMethod<ConfigType> {
	<DataType>(data: RequestData<ConfigType>): Promise<DataType>;
}

@injectable()
export abstract class BaseApi<RequestConfigType> {
	protected abstract _get: ApiMethod<RequestConfigType>;
	protected abstract _post: ApiMethod<RequestConfigType>;
	protected abstract _put: ApiMethod<RequestConfigType>;
	protected abstract _delete: ApiMethod<RequestConfigType>;
}
