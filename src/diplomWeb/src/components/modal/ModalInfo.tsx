import { Box, Modal } from '@mui/material';
import { grey } from '@mui/material/colors';
import { observer } from 'mobx-react-lite';
import { BaseSyntheticEvent, useEffect } from 'react';
import { useFormContext } from 'react-hook-form';
import { useInject } from '../../hooks/useInject';
import { Types } from '../../inversify/inversify.types';
import { IData } from '../../models/interfaces/IData';
import { dataInfoResponse } from '../../models/interfaces/response/dataInfoResponse';
import GeneralStore from '../../store/GeneralStore';
import TableDataStore from '../../store/base/TableDataStore';

export interface IModalInfo<IItem extends IData, IInfoResponse extends dataInfoResponse> {
	onFormSubmit: (e?: BaseSyntheticEvent | undefined) => Promise<void>;
	children?: JSX.Element;
	store: TableDataStore<IItem, IInfoResponse>;
}

const ModalInfo = <IItem extends IData, IInfoResponse extends dataInfoResponse>(
	props: IModalInfo<IItem, IInfoResponse>,
): JSX.Element => {
	const generalStore = useInject<GeneralStore>(Types.GeneralStore);

	const methods = useFormContext();
	const { modalInfo } = props.store;

	const resetData = (): void => {
		methods.reset();
		modalInfo.resetTableDataInfo();
		modalInfo.errorStore.resetError();
	};

	useEffect(() => {
		if (modalInfo.tableDataInfoId !== '' && !modalInfo.tableDataInfo) {
			props.store.getTableDataInfo();
		}
		return () => {
			resetData();
		};
	}, []);

	return (
		<Modal
			open={modalInfo.modalStore.modalActive}
			onClose={(): void => modalInfo.modalStore.handleClose()}
			sx={{ display: 'flex', alignItems: 'center', justifyContent: 'center', outline: '0' }}
		>
			<Box
				component={'form'}
				sx={{
					bgcolor: generalStore.themeMode === 'dark' ? grey[900] : 'white',
					borderRadius: '4px',
					width: '550px',
					gap: '16px',
					display: 'flex',
					flexDirection: 'column',
					padding: '16px',
					position: 'relative',
				}}
				onSubmit={props.onFormSubmit}
			>
				{props.children}
			</Box>
		</Modal>
	);
};

export default observer(ModalInfo);
