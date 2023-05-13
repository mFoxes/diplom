import { Delete, Edit, ListAlt, QrCode } from '@mui/icons-material';
import {
	Box,
	Button,
	IconButton,
	Paper,
	Table,
	TableBody,
	TableCell,
	TableContainer,
	TableHead,
	TableRow,
} from '@mui/material';
import { observer } from 'mobx-react-lite';
import { useEffect } from 'react';
import { DownloadableImage } from '../../components/UI/img/DownloadableImage';
import { InputFilter } from '../../components/UI/input/InputFilter';
import { ListPagination } from '../../components/UI/pagination/ListPagination';
import { TableHeadCell } from '../../components/tableHeadCell/TableHeadCell';

import { useInject } from '../../hooks/useInject';
import devicesPhotoEmpty from '../../img/devicePhotoEmpty.png';
import { Types } from '../../inversify/inversify.types';
import { IDevices } from '../../models/interfaces/IDevices';
import DevicesStore from '../../store/DevicesStore';
import GeneralStore from '../../store/GeneralStore';
import { createQrCode, nameof } from '../../utilities/Utilities';
import { DeviceHistory } from '../modals/deviceHistory/DeviceHistory';
import { DeviceInfo } from '../modals/deviceInfo/DeviceInfo';
import { DeleteModal } from '../modals/general/DeleteModal';

const DevicesPage = (): JSX.Element => {
	const generalStore = useInject<GeneralStore>(Types.GeneralStore);
	const devicesStore = useInject<DevicesStore>(Types.DevicesStore);
	const { tableDataStore: devicesTableStore } = devicesStore;

	const { params, modalInfo, modalConfirm, modalDeviceHistory } = devicesTableStore;

	const { skip, take, orderBy, orderDir } = params;

	useEffect(() => {
		generalStore.setPageTitle('Устройства');
		params.resetTableParams();
	}, []);

	useEffect((): void => {
		devicesTableStore.updateTableData();
	}, [skip, take, orderBy, orderDir]);

	return (
		<Box
			sx={{
				display: 'flex',
				flexDirection: 'column',
				alignItems: 'center',
				justifyContent: 'center',
				width: '100%',
				height: '100%',
			}}
		>
			<Box
				sx={{
					display: 'flex',
					flexDirection: 'column',
					alignItems: 'center',
					width: '100%',
					height: '100%',
					gap: '20px',
				}}
			>
				<Box
					sx={{
						display: 'flex',
						flexDirection: 'column',
						width: '100%',
					}}
				>
					<Box>
						<Button onClick={(): void => modalInfo.modalStore.handleOpen()}>Добавить</Button>
					</Box>
					<Box
						sx={{
							display: 'flex',
							alignItems: 'center',
							gap: '40px',
							flexGrow: '1',
							justifyContent: 'right',
							width: '100%',
						}}
					>
						<InputFilter
							inputName={nameof<IDevices>('InventoryNumber')}
							placeholder={'Поиск по инвентарному номеру'}
							store={devicesTableStore}
						/>
						<InputFilter
							inputName={nameof<IDevices>('Name')}
							placeholder={'Поиск по названию'}
							store={devicesTableStore}
						/>
					</Box>
				</Box>
				<TableContainer component={Paper} sx={{ width: '100%', display: 'flex' }}>
					<Table sx={{ minWidth: 650 }} aria-label='simple table'>
						<TableHead>
							<TableRow>
								<TableCell width={'120px'} sx={{ fontSize: '16px' }}>
									Фото
								</TableCell>
								<TableCell>
									<TableHeadCell
										fieldName={nameof<IDevices>('InventoryNumber')}
										text='Инвентарный номер'
										paramsStore={params}
									/>
								</TableCell>
								<TableCell>
									<TableHeadCell
										fieldName={nameof<IDevices>('Name')}
										text='Наименование'
										paramsStore={params}
									/>
								</TableCell>
								<TableCell align='center' sx={{ fontSize: '16px' }} width={'150px'}>
									Изменить
								</TableCell>
							</TableRow>
						</TableHead>
						<TableBody>
							{devicesTableStore.items &&
								devicesTableStore.items.map((item, indx) => (
									<TableRow
										key={item.Name + indx}
										sx={{ '&:last-child td, &:last-child th': { border: 0 }, maxHeight: '20px' }}
									>
										<TableCell component='th' scope='row'>
											<DownloadableImage
												photoId={item.PhotoId}
												emptyPhoto={devicesPhotoEmpty}
												style={{ width: '80px', height: '80px', objectFit: 'cover' }}
												alt='device'
											/>
										</TableCell>
										<TableCell component='th' scope='row'>
											{item.InventoryNumber}
										</TableCell>
										<TableCell component='th' scope='row'>
											{item.Name}
										</TableCell>
										<TableCell component='th' scope='row' align='center'>
											<IconButton
												onClick={(): void => {
													modalInfo.modalStore.handleOpen();
													modalInfo.setTableDataInfoId(item.Id);
												}}
											>
												<Edit />
											</IconButton>
											<IconButton
												onClick={(): void => {
													devicesTableStore.modalConfirm.setItem(item);
													devicesTableStore.modalConfirm.modalStore.handleOpen();
												}}
											>
												<Delete />
											</IconButton>

											<IconButton
												onClick={(): void => {
													createQrCode(item.InventoryNumber);
												}}
											>
												<QrCode />
											</IconButton>

											<IconButton
												onClick={(): void => {
													devicesTableStore.getDeviceHistory(item.Id);
													devicesTableStore.modalDeviceHistory.modalStore.handleOpen();
												}}
											>
												<ListAlt />
											</IconButton>
										</TableCell>
									</TableRow>
								))}
						</TableBody>
					</Table>
				</TableContainer>
			</Box>
			{modalInfo.modalStore.modalActive ? <DeviceInfo /> : ''}
			{modalConfirm.modalStore.modalActive ? <DeleteModal /> : ''}
			{modalDeviceHistory.modalStore.modalActive ? <DeviceHistory deviceHistoryStore={modalDeviceHistory} /> : ''}
			<ListPagination store={devicesTableStore} />
		</Box>
	);
};

export default observer(DevicesPage);
