import { Box, Button, Typography } from '@mui/material';
import { observer } from 'mobx-react-lite';
import { CancelButton } from '../../../components/UI/cancelButton/CancelButton';
import { ModalConfirm } from '../../../components/modal/ModalConfirm';
import { useInject } from '../../../hooks/useInject';
import { Types } from '../../../inversify/inversify.types';
import IDashboard from '../../../models/interfaces/IDashboard';
import DashboardStore from '../../../store/DashboardStore';

export const ReturnModal = observer((): JSX.Element => {
	const dashboardStore = useInject<DashboardStore>(Types.DashboardStore);
	const { tableDataStore: dashboardTableStore } = dashboardStore;

	const { modalConfirm } = dashboardTableStore;

	const getChangeData = (): IDashboard | undefined => {
		const data = { ...modalConfirm.item } as IDashboard;

		if (data) {
			data.State = 'free';
		}

		return data;
	};

	const onSubmit = async (): Promise<void> => {
		const returnData = getChangeData();

		if (returnData) {
			dashboardTableStore.updateTableInfo(returnData);
		}
		modalConfirm.modalStore.handleClose();
	};

	return (
		<ModalConfirm confirmStore={dashboardTableStore.modalConfirm}>
			<>
				<Box sx={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
					<Typography>Вы действительно хотите вернуть устройство?</Typography>
				</Box>

				<Box sx={{ display: 'flex', gap: '20px', justifyContent: 'right', alignItems: 'center' }}>
					<CancelButton modalConfirm={modalConfirm}>
						<>Отменить</>
					</CancelButton>
					<Button onClick={onSubmit} variant='contained'>
						Вернуть
					</Button>
				</Box>
			</>
		</ModalConfirm>
	);
});
