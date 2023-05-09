import { observer } from 'mobx-react-lite';
import { useEffect, useState } from 'react';
import { useInject } from '../../../hooks/useInject';
import { Types } from '../../../inversify/inversify.types';
import { IDownloadableImage } from '../../../models/interfaces/IDownloadableImage';
import PhotosCacheStore from '../../../store/base/helpers/PhotosCacheStore';

export const DownloadableImage = observer(({ photoId, emptyPhoto, ...props }: IDownloadableImage): JSX.Element => {
	const photosCacheStore = useInject<PhotosCacheStore>(Types.PhotosCacheStore);

	const [photo, setPhoto] = useState<string>(emptyPhoto);

	const chooseImage = async (): Promise<void> => {
		if (photoId) {
			const photoCache = photosCacheStore.getPhotoByKey(photoId);

			if (photoCache) {
				setPhoto(photoCache);
			} else {
				const photo = await photosCacheStore.downloadImage(photoId);
				if (photo) {
					setPhoto(photo);
				}
			}
		} else {
			setPhoto(emptyPhoto);
		}
	};

	useEffect((): void => {
		chooseImage();
	}, [photoId]);

	return <img src={photo} {...props} />;
});
