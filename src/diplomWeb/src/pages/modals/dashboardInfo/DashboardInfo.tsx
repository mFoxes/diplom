import { yupResolver } from '@hookform/resolvers/yup';
import { Close } from '@mui/icons-material';
import { Box, Button, IconButton, MenuItem, Typography } from '@mui/material';
import { LocalizationProvider } from '@mui/x-date-pickers';
import { AdapterDateFns } from '@mui/x-date-pickers/AdapterDateFns';
import { observer } from 'mobx-react-lite';
import { useEffect } from 'react';
import { FormProvider, useForm } from 'react-hook-form';
import ModalInfo from '../../../components/modal/ModalInfo';
import { ModalDateField } from '../../../components/modalField/ModalDateField';
import { ModalSelectField } from '../../../components/modalField/ModalSelectField';
import { ModalTextField } from '../../../components/modalField/ModalTextField';
import { IDashboardInfoResponse } from '../../../models/interfaces/response/IDashboardResponse';
import { DashboardInfoSchema } from '../../../models/schemas/DashboardSchema';
import DashboardStore from '../../../store/DashboardStore';
import { nameof } from '../../../utilities/Utilities';
import { useInject } from '../../../hooks/useInject';
import { Types } from '../../../inversify/inversify.types';

export const DashboardInfo = observer((): JSX.Element => {
	const dashboardStore = useInject<DashboardStore>(Types.DashboardStore);

	const { modalInfo } = dashboardStore;

	const methods = useForm<IDashboardInfoResponse>({
		mode: 'all',
		resolver: yupResolver(DashboardInfoSchema),
	});

	const changeNewData = async (data: IDashboardInfoResponse, changeData: IDashboardInfoResponse): Promise<void> => {
		changeData.State = 'booked';

		if (data.UserId) {
			changeData.UserId = data.UserId;
		}

		changeData.TakeAt = data.TakeAt;
		changeData.TakeAt.setHours(3, 0, 0);
		changeData.ReturnAt = data.ReturnAt;
		changeData.ReturnAt.setHours(23, 59, 59);
	};

	const onSubmit = async (data: IDashboardInfoResponse): Promise<void> => {
		const originDashboardInfo = { ...modalInfo.tableDataInfo } as IDashboardInfoResponse;

		if (originDashboardInfo) {
			await changeNewData(data, originDashboardInfo);

			await dashboardStore.updateTableInfo(originDashboardInfo);
		}
	};

	const loadDataInfo = async (): Promise<void> => {
		await dashboardStore.getTableDataInfo();

		if (modalInfo.tableDataInfo?.State === 'free') {
			await dashboardStore.getAllEmployeeNames();
		}
	};

	useEffect(() => {
		loadDataInfo();
	}, []);

	useEffect(() => {
		methods.reset({
			Name: modalInfo.tableDataInfo?.Name,
		});
		if (modalInfo.tableDataInfo?.TakeAt && modalInfo.tableDataInfo?.ReturnAt) {
			methods.reset({
				TakeAt: new Date(modalInfo.tableDataInfo?.TakeAt),
				ReturnAt: new Date(modalInfo.tableDataInfo?.ReturnAt),
			});
		} else {
			methods.reset({
				TakeAt: new Date(),
				ReturnAt: new Date(),
			});
		}
	}, [modalInfo.tableDataInfo?.Name, modalInfo.tableDataInfo?.TakeAt, modalInfo.tableDataInfo?.ReturnAt]);

	return (
		<FormProvider {...methods}>
			<ModalInfo onFormSubmit={methods.handleSubmit(onSubmit)} store={dashboardStore}>
				<>
					<IconButton
						onClick={(): void => {
							modalInfo.modalStore.handleClose();
						}}
						sx={{ padding: '16px', position: 'absolute', top: '0', right: '0' }}
					>
						<Close />
					</IconButton>

					<Box sx={{ width: '100%', paddingBottom: '16px', paddingTop: '16px' }}>
						{modalInfo.tableDataInfo?.State === 'free' ? (
							<Typography variant='h5'>Создание брони</Typography>
						) : (
							<Typography variant='h5'>Редактирование брони</Typography>
						)}
					</Box>

					<Box sx={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
						<LocalizationProvider dateAdapter={AdapterDateFns}>
							{modalInfo.tableDataInfo?.State === 'free' ? (
								<ModalSelectField
									fieldName={'Сотрудник'}
									selectInputAttribute={{
										inputName: nameof<IDashboardInfoResponse>('UserId'),
										serverErrorList: modalInfo.errorStore.error,
									}}
								>
									{dashboardStore.employees.map(
										(item): JSX.Element => (
											<MenuItem key={item.Id} value={item.Id}>
												{item.Name}
											</MenuItem>
										),
									)}
								</ModalSelectField>
							) : (
								<ModalTextField fieldName={'Сотрудник'} fieldText={modalInfo.tableDataInfo?.TakedBy} />
							)}

							<ModalDateField
								fieldName={'Дата получения'}
								inputAttribute={{
									inputName: nameof<IDashboardInfoResponse>('TakeAt'),
									serverErrorStore: modalInfo.errorStore,
									hasErrorField: true,
								}}
							/>

							<ModalDateField
								fieldName={'Дата возврата'}
								inputAttribute={{
									inputName: nameof<IDashboardInfoResponse>('ReturnAt'),
									serverErrorStore: modalInfo.errorStore,
									hasErrorField: true,
								}}
							/>
						</LocalizationProvider>
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
