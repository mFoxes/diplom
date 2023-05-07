import { Box, Button, Typography } from '@mui/material';
import { observer } from 'mobx-react-lite';
import { CancelButton } from '../../../components/UI/cancelButton/CancelButton';
import { ModalConfirm } from '../../../components/modal/ModalConfirm';
import { useInject } from '../../../hooks/useInject';
import { Types } from '../../../inversify/inversify.types';
import { IDevices } from '../../../models/interfaces/IDevices';
import DevicesStore from '../../../store/DevicesStore';
import { getErrorListByName, nameof } from '../../../utilities/Utilities';

export const DeleteModal = observer((): JSX.Element => {
	const devicesStore = useInject<DevicesStore>(Types.DevicesStore);

	const { modalConfirm } = devicesStore;

	const errorList = getErrorListByName(nameof<IDevices>('Id'), modalConfirm.errorStore.error);

	const onSubmit = async (): Promise<void> => {
		if (modalConfirm.item?.Id) {
			devicesStore.deleteTableData(modalConfirm.item?.Id);
		}
	};

	return (
		<ModalConfirm confirmStore={devicesStore.modalConfirm}>
			<>
				<Box sx={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
					<Typography>Вы действительно хотите удалить устройство?</Typography>
				</Box>

				<Box>
					<Typography sx={{ minHeight: '30px', color: '#d32f2f' }}>{errorList}</Typography>
				</Box>

				<Box sx={{ display: 'flex', gap: '20px', justifyContent: 'right', alignItems: 'center' }}>
					<CancelButton modalConfirm={modalConfirm}>
						<>Отменить</>
					</CancelButton>
					<Button onClick={onSubmit} variant='contained'>
						Удалить
					</Button>
				</Box>
			</>
		</ModalConfirm>
	);
});
