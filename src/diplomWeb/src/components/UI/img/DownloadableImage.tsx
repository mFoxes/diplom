import { observer } from 'mobx-react-lite';
import { useContext, useEffect, useState } from 'react';
import { Context } from '../../..';
import { IDownloadableImage } from '../../../models/interfaces/IDownloadableImage';
import DownloadableImageService from '../../../service/DownloadableImgService';

export const DownloadableImage = observer(({ photoId, emptyPhoto, ...props }: IDownloadableImage): JSX.Element => {
	const { photosCacheStore } = useContext(Context);

	const [photo, setPhoto] = useState<string>(emptyPhoto);

	const chooseImage = async (): Promise<void> => {
		if (photoId) {
			const photoCache = photosCacheStore.getPhotoByKey(photoId);

			if (photoCache) {
				setPhoto(photoCache);
			} else {
				downloadImage(photoId);
			}
		} else {
			setPhoto(emptyPhoto);
		}
	};

	const downloadImage = async (photoId: string): Promise<void> => {
		const res = await DownloadableImageService.getPhotoById(photoId);

		if (res?.data.size !== 0 && res) {
			const photo = URL.createObjectURL(res?.data);
			setPhoto(photo);
			photosCacheStore.setPhotoByKey(photoId, photo);
		}
	};

	useEffect((): void => {
		chooseImage();
	}, [photoId]);

	return <img src={photo} {...props} />;
});
