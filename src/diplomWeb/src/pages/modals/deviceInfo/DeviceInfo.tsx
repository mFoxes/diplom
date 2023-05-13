import { yupResolver } from '@hookform/resolvers/yup';
import { Close } from '@mui/icons-material';
import { Box, Button, IconButton, Typography } from '@mui/material';
import { useEffect } from 'react';
import { FormProvider, useForm } from 'react-hook-form';
import ModalInfo from '../../../components/modal/ModalInfo';
import { ModalImgField } from '../../../components/modalField/ModalImgField';
import { ModalInputField } from '../../../components/modalField/ModalInputField';
import { IDevicesInfoResponse } from '../../../models/interfaces/response/IDevicesInfoResponse';
import { DeviceInfoSchema } from '../../../models/schemas/DeviceInfoSchema';
import { nameof } from '../../../utilities/Utilities';

import { observer } from 'mobx-react-lite';
import { useInject } from '../../../hooks/useInject';
import deviceEmptyPhoto from '../../../img/devicePhotoEmpty.png';
import { Types } from '../../../inversify/inversify.types';
import DevicesStore from '../../../store/DevicesStore';

export const DeviceInfo = observer((): JSX.Element => {
	const devicesStore = useInject<DevicesStore>(Types.DevicesStore);
	const { tableDataStore: devicesTableStore } = devicesStore;

	const { modalInfo } = devicesTableStore;

	const methods = useForm<IDevicesInfoResponse>({
		mode: 'all',
		resolver: yupResolver(DeviceInfoSchema),
	});

	const chooseSaveMethod = async (originDeviceInfo: IDevicesInfoResponse): Promise<void> => {
		if (modalInfo.tableDataInfoId === '') {
			await devicesTableStore.addNewTableInfo(originDeviceInfo);
		} else if (originDeviceInfo.Id) {
			await devicesTableStore.updateTableInfo(originDeviceInfo);
		}
	};

	const chooseData = (originDeviceInfo: IDevicesInfoResponse | undefined): IDevicesInfoResponse => {
		let changeData: IDevicesInfoResponse;

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

	const changeNewData = async (data: IDevicesInfoResponse, changeData: IDevicesInfoResponse): Promise<void> => {
		changeData.InventoryNumber = data.InventoryNumber;
		changeData.Name = data.Name;
	};

	const onSubmit = async (data: IDevicesInfoResponse): Promise<void> => {
		const originDeviceInfo = { ...modalInfo.tableDataInfo } as IDevicesInfoResponse;

		const changeData = chooseData(originDeviceInfo);

		await changeNewData(data, changeData);

		await devicesTableStore.changePhoto(
			methods,
			nameof<IDevicesInfoResponse>('Photo'),
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
			<ModalInfo onFormSubmit={methods.handleSubmit(onSubmit)} store={devicesTableStore}>
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
									inputName: nameof<IDevicesInfoResponse>('Photo'),
									accept: '.jpg,.jpeg,.png',
									serverErrorStore: modalInfo.errorStore,
								},
							}}
						/>

						<ModalInputField
							fieldName={'Инвентарный номер'}
							inputAttribute={{
								inputName: nameof<IDevicesInfoResponse>('InventoryNumber'),
								serverErrorStore: modalInfo.errorStore,
								hasErrorField: true,
							}}
						/>

						<ModalInputField
							fieldName={'Наименование'}
							inputAttribute={{
								inputName: nameof<IDevicesInfoResponse>('Name'),
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
