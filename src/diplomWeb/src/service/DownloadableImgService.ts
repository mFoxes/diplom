import { AxiosResponse } from 'axios';
import { $api } from '../http';

export default class DownloadableImageService {
	static async getPhotoById(photoId: string): Promise<AxiosResponse<File> | undefined> {
		try {
			return $api.get(`files/${photoId}`, { responseType: 'blob' });
		} catch (e) {
			console.log(e);
		}
	}
}
