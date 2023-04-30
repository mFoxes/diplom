import { ImgHTMLAttributes } from 'react';

export interface IDownloadableImage extends ImgHTMLAttributes<HTMLImageElement> {
	photoId?: string;
	emptyPhoto: string;
}
