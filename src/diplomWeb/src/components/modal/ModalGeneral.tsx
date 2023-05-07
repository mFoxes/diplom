import { Modal, Paper } from '@mui/material';
import { Box } from '@mui/system';
import { observer } from 'mobx-react-lite';
import ModalStore from '../../store/base/ModalStore';

export interface IModalGeneral {
	children?: JSX.Element;
	modalStore: ModalStore;
}

export const ModalGeneral = observer(({ modalStore, ...props }: IModalGeneral): JSX.Element => {
	return (
		<Modal
			open={modalStore.modalActive}
			onClose={(): void => modalStore.handleClose()}
			sx={{ display: 'flex', alignItems: 'center', justifyContent: 'center', outline: '0' }}
		>
			<Box
				component={Paper}
				sx={{
					borderRadius: '4px',
					minWidth: '550px',
					width: 'fit-content',
					gap: '16px',
					display: 'flex',
					flexDirection: 'column',
					padding: '16px',
					position: 'relative',
				}}
			>
				{props.children}
			</Box>
		</Modal>
	);
});
