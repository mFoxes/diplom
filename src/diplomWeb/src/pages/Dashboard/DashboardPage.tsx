import { Edit, KeyboardReturn, ListAlt } from '@mui/icons-material';
import {
	Box,
	IconButton,
	Paper,
	Table,
	TableBody,
	TableCell,
	TableContainer,
	TableHead,
	TableRow,
	Typography,
} from '@mui/material';
import { observer } from 'mobx-react-lite';
import { useEffect } from 'react';
import { InputFilter } from '../../components/UI/input/InputFilter';
import { ListPagination } from '../../components/UI/pagination/ListPagination';
import { DashboardStatus } from '../../components/dashboardStatus/DashboardStatus';
import { TableHeadCell } from '../../components/tableHeadCell/TableHeadCell';
import { useInject } from '../../hooks/useInject';
import { Types } from '../../inversify/inversify.types';
import IDashboard from '../../models/interfaces/IDashboard';
import AuthStore from '../../store/AuthStore';
import DashboardStore from '../../store/DashboardStore';
import GeneralStore from '../../store/GeneralStore';
import { nameof } from '../../utilities/Utilities';
import { DashboardInfo } from '../modals/dashboardInfo/DashboardInfo';
import { DeviceHistory } from '../modals/deviceHistory/DeviceHistory';
import { ReturnModal } from '../modals/general/ReturnModal';

export const DashboardPage = observer((): JSX.Element => {
	const dashboardStore = useInject<DashboardStore>(Types.DashboardStore);
	const generalStore = useInject<GeneralStore>(Types.GeneralStore);
	const authStore = useInject<AuthStore>(Types.AuthStore);

	const { params, modalInfo, modalConfirm, modalDeviceHistory } = dashboardStore;

	const { skip, take, orderBy, orderDir } = params;

	useEffect(() => {
		generalStore.setPageTitle('Дашборд');
	}, []);

	useEffect((): void => {
		dashboardStore.updateTableData();
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
				<Box sx={{ display: 'flex', gap: '40px', justifyContent: 'center', width: '100%' }}>
					<InputFilter
						inputName={nameof<IDashboard>('InventoryNumber')}
						placeholder={'Поиск по инвентарному номеру'}
						store={dashboardStore}
					/>
					<InputFilter
						inputName={nameof<IDashboard>('Name')}
						placeholder={'Поиск по названию устройства'}
						store={dashboardStore}
					/>
					<InputFilter
						inputName={nameof<IDashboard>('TakedBy')}
						placeholder={'Поиск по имени сотрудника'}
						store={dashboardStore}
					/>
				</Box>
				<TableContainer component={Paper} sx={{ width: '100%', display: 'flex' }}>
					<Table sx={{ minWidth: 650 }} aria-label='simple table'>
						<TableHead>
							<TableRow>
								<TableCell>
									<TableHeadCell
										fieldName={nameof<IDashboard>('InventoryNumber')}
										text='Устройство'
										paramsStore={params}
									/>
								</TableCell>
								<TableCell>
									<TableHeadCell
										fieldName={nameof<IDashboard>('Name')}
										text='Название'
										paramsStore={params}
									/>
								</TableCell>
								<TableCell>
									<TableHeadCell
										fieldName={nameof<IDashboard>('State')}
										text='Состояние'
										paramsStore={params}
									/>
								</TableCell>
								<TableCell align='center' sx={{ fontSize: '16px' }} width={'150px'}>
									Изменить
								</TableCell>
							</TableRow>
						</TableHead>
						<TableBody>
							{dashboardStore.items &&
								dashboardStore.items.map((item, indx) => (
									<TableRow
										key={item.Name + indx}
										sx={{
											'&:last-child td, &:last-child th': { border: 0 },
										}}
									>
										<TableCell component='th' scope='row'>
											<Typography>{item.InventoryNumber}</Typography>
										</TableCell>

										<TableCell component='th' scope='row'>
											<Typography>{item.Name}</Typography>
										</TableCell>

										<DashboardStatus deviceItem={item} />

										<TableCell
											component='th'
											scope='row'
											align='center'
											width={'200px'}
											height={'85px'}
										>
											{authStore.isAdmin && (
												<>
													<IconButton
														onClick={(): void => {
															modalInfo.setTableDataInfoId(item.Id);
															modalInfo.modalStore.handleOpen();
														}}
													>
														<Edit />
													</IconButton>

													<IconButton
														onClick={(): void => {
															modalConfirm.setItem(item);
															modalConfirm.modalStore.handleOpen();
														}}
														disabled={item.State === 'free'}
													>
														<KeyboardReturn />
													</IconButton>
												</>
											)}

											<IconButton
												onClick={(): void => {
													dashboardStore.getDeviceHistory(item.DeviceId);
													dashboardStore.modalDeviceHistory.modalStore.handleOpen();
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
			<ListPagination store={dashboardStore} />
			{modalInfo.modalStore.modalActive ? <DashboardInfo /> : ''}
			{modalConfirm.modalStore.modalActive ? <ReturnModal /> : ''}
			{modalDeviceHistory.modalStore.modalActive ? <DeviceHistory deviceHistoryStore={modalDeviceHistory} /> : ''}
		</Box>
	);
});
