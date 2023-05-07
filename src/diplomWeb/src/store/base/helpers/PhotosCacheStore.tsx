import { makeAutoObservable } from 'mobx';

interface IPhotoCache {
	[key: string]: string;
}

export default class PhotosCacheStore {
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
}
