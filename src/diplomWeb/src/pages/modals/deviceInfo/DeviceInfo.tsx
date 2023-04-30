import { yupResolver } from '@hookform/resolvers/yup';
import { Close } from '@mui/icons-material';
import { Box, Button, IconButton, Typography } from '@mui/material';
import { useContext, useEffect } from 'react';
import { FormProvider, useForm } from 'react-hook-form';
import { Context } from '../../..';
import ModalInfo from '../../../components/modal/ModalInfo';
import { ModalImgField } from '../../../components/modalField/ModalImgField';
import { ModalInputField } from '../../../components/modalField/ModalInputField';
import { devicesInfoResponse } from '../../../models/interfaces/response/devicesInfoResponse';
import { DeviceInfoSchema } from '../../../models/schemas/DeviceInfoSchema';
import { nameof } from '../../../utilities/Utilities';

import { observer } from 'mobx-react-lite';
import deviceEmptyPhoto from '../../../img/devicePhotoEmpty.png';

export const DeviceInfo = observer((): JSX.Element => {
	const { devicesStore } = useContext(Context);

	const { modalInfo } = devicesStore;

	const methods = useForm<devicesInfoResponse>({
		mode: 'all',
		resolver: yupResolver(DeviceInfoSchema),
	});

	const chooseSaveMethod = async (originDeviceInfo: devicesInfoResponse): Promise<void> => {
		if (modalInfo.tableDataInfoId === '') {
			await devicesStore.addNewTableInfo(originDeviceInfo);
		} else if (originDeviceInfo.Id) {
			await devicesStore.updateTableInfo(originDeviceInfo);
		}
	};

	const chooseData = (originDeviceInfo: devicesInfoResponse | undefined): devicesInfoResponse => {
		let changeData: devicesInfoResponse;

		if (modalInfo.tableDataInfoId !== '' && originDeviceInfo) {
			changeData = originDeviceInfo;
		} else {
			changeData = {
				Name: '',
				InventoryNumber: '',
				PhotoId: '',
			};
		}

		return changeData;
	};

	const changeNewData = async (data: devicesInfoResponse, changeData: devicesInfoResponse): Promise<void> => {
		changeData.InventoryNumber = data.InventoryNumber;
		changeData.Name = data.Name;
	};

	const onSubmit = async (data: devicesInfoResponse): Promise<void> => {
		const originDeviceInfo = { ...modalInfo.tableDataInfo } as devicesInfoResponse;

		const changeData = chooseData(originDeviceInfo);

		await changeNewData(data, changeData);

		await devicesStore.changePhoto(
			methods,
			nameof<devicesInfoResponse>('Photo'),
			data.Photo,
			changeData,
			chooseSaveMethod,
		);
	};

	useEffect(() => {
		methods.reset({
			InventoryNumber: modalInfo.tableDataInfo?.InventoryNumber,
			Name: modalInfo.tableDataInfo?.Name,
		});
	}, [modalInfo.tableDataInfo?.InventoryNumber, modalInfo.tableDataInfo?.Name]);

	return (
		<FormProvider {...methods}>
			<ModalInfo onFormSubmit={methods.handleSubmit(onSubmit)} store={devicesStore}>
				<>
					<IconButton
						onClick={(): void => {
							modalInfo.modalStore.handleClose();
						}}
						sx={{ padding: '16px', position: 'absolute', top: '0', right: '0' }}
					>
						<Close />
					</IconButton>

					<Box sx={{ width: '100%' }}>
						{modalInfo.tableDataInfoId === '' ? (
							<Typography variant='h5'>Создание устройства</Typography>
						) : (
							<Typography variant='h5'>Редактирование устройства</Typography>
						)}
					</Box>

					<Box sx={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
						<ModalImgField
							modalActive={modalInfo.modalStore.modalActive}
							photo={{
								photoId: modalInfo.tableDataInfo?.PhotoId,
								photoEmpty: deviceEmptyPhoto,
								inputFileAttribute: {
									inputName: nameof<devicesInfoResponse>('Photo'),
									accept: '.jpg,.jpeg,.png',
									serverErrorStore: modalInfo.errorStore,
								},
							}}
						/>

						<ModalInputField
							fieldName={'Инвентарный номер'}
							inputAttribute={{
								inputName: nameof<devicesInfoResponse>('InventoryNumber'),
								serverErrorStore: modalInfo.errorStore,
								hasErrorField: true,
							}}
						/>

						<ModalInputField
							fieldName={'Наименование'}
							inputAttribute={{
								inputName: nameof<devicesInfoResponse>('Name'),
								serverErrorStore: modalInfo.errorStore,
								hasErrorField: true,
							}}
						/>
					</Box>

					<Box sx={{ display: 'flex', justifyContent: 'right', alignItems: 'center' }}>
						<Button type='submit' variant='contained'>
							Сохранить
						</Button>
					</Box>
				</>
			</ModalInfo>
		</FormProvider>
	);
});
