import { Box, Button, Typography } from '@mui/material';
import { observer } from 'mobx-react-lite';
import { ModalConfirm } from '../../../components/modal/ModalConfirm';
import { CancelButton } from '../../../components/UI/cancelButton/CancelButton';
import AuthStore from '../../../store/AuthStore';
import { useInject } from '../../../hooks/useInject';
import { Types } from '../../../inversify/inversify.types';

export const ExitModal = observer((): JSX.Element => {
	const authStore = useInject<AuthStore>(Types.AuthStore);

	const { modalConfirm } = authStore;

	const onSubmit = async (): Promise<void> => {
		authStore.logout();
		modalConfirm.modalStore.handleClose();
	};

	return (
		<ModalConfirm confirmStore={authStore.modalConfirm}>
			<>
				<Box sx={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
					<Typography>Вы действительно хотите выйти?</Typography>
				</Box>

				<Box sx={{ display: 'flex', gap: '20px', justifyContent: 'right', alignItems: 'center' }}>
					<CancelButton modalConfirm={modalConfirm}>
						<>Отменить</>
					</CancelButton>
					<Button onClick={onSubmit} variant='contained'>
						Выход
					</Button>
				</Box>
			</>
		</ModalConfirm>
	);
});
