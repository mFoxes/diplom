import { yupResolver } from '@hookform/resolvers/yup';
import { Close } from '@mui/icons-material';
import { Box, Button, IconButton, Typography } from '@mui/material';
import { observer } from 'mobx-react-lite';
import { useEffect } from 'react';
import { FormProvider, useForm } from 'react-hook-form';
import ModalInfo from '../../../components/modal/ModalInfo';
import { ModalImgField } from '../../../components/modalField/ModalImgField';
import { ModalInputField } from '../../../components/modalField/ModalInputField';

import { useInject } from '../../../hooks/useInject';
import employeePhotoEmpty from '../../../img/employeePhotoEmpty.png';
import { Types } from '../../../inversify/inversify.types';
import { employeeInfoResponse } from '../../../models/interfaces/response/employeeInfoResponse';
import { EmployeeInfoSchema } from '../../../models/schemas/EmployeeInfoSchema';
import AuthStore from '../../../store/AuthStore';
import EmployeeStore from '../../../store/EmployeesStore';
import { nameof } from '../../../utilities/Utilities';

const EmployeeInfo = (): JSX.Element => {
	const employeesStore = useInject<EmployeeStore>(Types.EmployeeStore);
	const authStore = useInject<AuthStore>(Types.AuthStore);

	const { modalInfo } = employeesStore;

	const methods = useForm<employeeInfoResponse>({
		mode: 'all',
		resolver: yupResolver(EmployeeInfoSchema),
	});

	const changeNewData = async (data: employeeInfoResponse, changeData: employeeInfoResponse): Promise<void> => {
		changeData.Name = data.Name;
		changeData.MattermostName = data.MattermostName;
	};

	const chooseSaveMethod = async (originDeviceInfo: employeeInfoResponse): Promise<void> => {
		if (employeesStore.modalInfo.tableDataInfoId === authStore.currentEmployee?.Id) {
			employeesStore.updateTableInfo(originDeviceInfo, () => authStore.getCurrentEmployee());
		} else {
			employeesStore.updateTableInfo(originDeviceInfo);
		}
	};

	const onSubmit = async (data: employeeInfoResponse): Promise<void> => {
		const originEmployeeInfo = { ...modalInfo.tableDataInfo } as employeeInfoResponse;
		if (originEmployeeInfo) {
			changeNewData(data, originEmployeeInfo);

			await employeesStore.changePhoto(
				methods,
				nameof<employeeInfoResponse>('Photo'),
				data.Photo,
				originEmployeeInfo,
				chooseSaveMethod,
			);
		}
	};

	useEffect(() => {
		methods.reset({ Name: modalInfo.tableDataInfo?.Name, MattermostName: modalInfo.tableDataInfo?.MattermostName });
	}, [modalInfo.tableDataInfo?.Name, modalInfo.tableDataInfo?.MattermostName]);

	return (
		<FormProvider {...methods}>
			<ModalInfo onFormSubmit={methods.handleSubmit(onSubmit)} store={employeesStore}>
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
						<Typography variant='h5'>Редактирование пользователя</Typography>
					</Box>

					<Box sx={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
						<ModalImgField
							modalActive={modalInfo.modalStore.modalActive}
							photo={{
								photoId: modalInfo.tableDataInfo?.PhotoId,
								photoEmpty: employeePhotoEmpty,
								inputFileAttribute: {
									inputName: nameof<employeeInfoResponse>('Photo'),
									accept: '.jpg,.jpeg,.png',
									serverErrorStore: modalInfo.errorStore,
								},
							}}
						/>

						<ModalInputField
							fieldName={'Логин Маттермоста'}
							inputAttribute={{
								inputName: nameof<employeeInfoResponse>('MattermostName'),
								serverErrorStore: modalInfo.errorStore,
								hasErrorField: true,
							}}
						/>

						<ModalInputField
							fieldName={'ФИО'}
							hasColumnDirection
							inputAttribute={{
								inputName: nameof<employeeInfoResponse>('Name'),
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
};

export default observer(EmployeeInfo);
