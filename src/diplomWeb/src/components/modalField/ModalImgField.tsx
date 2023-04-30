import { Box } from '@mui/material';
import { observer } from 'mobx-react-lite';
import { useEffect, useState } from 'react';
import { DownloadableImage } from '../UI/img/DownloadableImage';
import { IInputFile, InputFile } from '../UI/input/InputFile';

export interface IModalImgField<T> {
	modalActive: boolean;
	photo: IModalPhoto<T>;
}

export interface IModalPhoto<T> {
	photoId?: string;
	photoEmpty: string;
	inputFileAttribute: IInputFile<T>;
}

export const ModalImgField = observer(<T,>(props: IModalImgField<T>): JSX.Element => {
	const [newPhoto, setNewPhoto] = useState<FileList | null>();

	useEffect(() => {
		setNewPhoto(undefined);
	}, [props.modalActive]);

	return (
		<Box>
			<Box sx={{ display: 'flex', justifyContent: 'space-between', width: '100%', gap: '32px' }}>
				{newPhoto ? (
					<img
						src={URL.createObjectURL(newPhoto[0])}
						alt='modalInfo'
						style={{ width: '250px', height: '250px', borderRadius: '4px', objectFit: 'cover' }}
					/>
				) : (
					<DownloadableImage
						photoId={props.photo.photoId}
						emptyPhoto={props.photo.photoEmpty}
						style={{ width: '250px', height: '250px', borderRadius: '4px', objectFit: 'cover' }}
						alt='modalInfo'
					/>
				)}
				<Box sx={{ width: '100%' }}>
					<InputFile setNewPhoto={setNewPhoto} {...props.photo.inputFileAttribute}>
						Загрузить фото
					</InputFile>
				</Box>
			</Box>
		</Box>
	);
});
