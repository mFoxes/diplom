import { injectable } from 'inversify';
import { AxiosApi } from './base/axiosApi';
import { Either } from '@sweet-monads/either';
import { AxiosResponse } from 'axios';
import { errorResponse } from '../../models/interfaces/response/errorResponse';

@injectable()
export default class DownloadableImageService extends AxiosApi {
	public async getPhotoById(photoId: string): Promise<Either<AxiosResponse<errorResponse[]>, File>> {
		const req = this._get<File>({ url: `files/${photoId}`, config: { responseType: 'blob' } });

		return this._doApiRequest(req);
	}
}
