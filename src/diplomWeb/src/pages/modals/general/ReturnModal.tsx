import { Box, Button, Typography } from '@mui/material';
import { observer } from 'mobx-react-lite';
import { useContext } from 'react';
import { Context } from '../../..';
import { ModalConfirm } from '../../../components/modal/ModalConfirm';
import { CancelButton } from '../../../components/UI/cancelButton/CancelButton';
import IDashboard from '../../../models/interfaces/IDashboard';

export const ReturnModal = observer((): JSX.Element => {
	const { dashboardStore } = useContext(Context);

	const { modalConfirm } = dashboardStore;

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
			dashboardStore.updateTableInfo(returnData);
		}
		modalConfirm.modalStore.handleClose();
	};

	return (
		<ModalConfirm confirmStore={dashboardStore.modalConfirm}>
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
