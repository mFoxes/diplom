import { inject, injectable } from 'inversify';
import { makeAutoObservable } from 'mobx';
import { Types } from '../../../inversify/inversify.types';
import DownloadableImageService from '../../../service/api/downloadableImgService';

interface IPhotoCache {
	[key: string]: string;
}

@injectable()
export default class PhotosCacheStore {
	@inject(Types.DownloadableImageService) private _downloadableImageService!: DownloadableImageService;

	private _photos: IPhotoCache = {};

	constructor() {
		makeAutoObservable(this);
	}

	public getPhotoByKey(key: string): string {
		return this._photos[key];
	}

	public setPhotoByKey(key: string, data: string): void {
		this._photos[key] = data;
	}

	public async downloadImage(photoId: string): Promise<string | void> {
		const res = await this._downloadableImageService.getPhotoById(photoId);

		if (res.isRight()) {
			const photo = URL.createObjectURL(res.value);
			this.setPhotoByKey(photoId, photo);

			return photo;
		}
	}
}
