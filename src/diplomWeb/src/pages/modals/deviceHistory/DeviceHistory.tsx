import { Close } from '@mui/icons-material';
import {
	TableContainer,
	Paper,
	Table,
	TableHead,
	TableRow,
	TableBody,
	TableCell,
	Typography,
	IconButton,
} from '@mui/material';
import { Box } from '@mui/system';
import { observer } from 'mobx-react-lite';
import { ModalGeneral } from '../../../components/modal/ModalGeneral';
import ModalDeviceHistoryStore from '../../../store/base/ModalDeviceHistoryStore';
import { getBookingTimeInterval } from '../../../utilities/Utilities';

interface IDeviceHistory {
	deviceHistoryStore: ModalDeviceHistoryStore;
}

export const DeviceHistory = observer(({ deviceHistoryStore, ...props }: IDeviceHistory): JSX.Element => {
	return (
		<ModalGeneral modalStore={deviceHistoryStore.modalStore}>
			<Box sx={{ paddingTop: '40px' }}>
				<IconButton
					onClick={(): void => {
						deviceHistoryStore.modalStore.handleClose();
					}}
					sx={{ padding: '16px', position: 'absolute', top: '0', right: '0' }}
				>
					<Close />
				</IconButton>
				<TableContainer
					component={Paper}
					sx={{ minWidth: 650, maxHeight: '600px', width: '100%', display: 'flex' }}
				>
					<Table stickyHeader aria-label='simple table'>
						<TableHead>
							<TableRow>
								<TableCell sx={{ fontSize: '16px' }}>Пользователь</TableCell>
								<TableCell sx={{ fontSize: '16px' }} align='center'>
									Период
								</TableCell>
							</TableRow>
						</TableHead>
						<TableBody>
							{deviceHistoryStore.deviceHistory &&
								deviceHistoryStore.deviceHistory.History.map((item, indx) => (
									<TableRow
										key={item.TakedBy + indx}
										sx={{ '&:last-child td, &:last-child th': { border: 0 }, maxHeight: 20 }}
									>
										<TableCell component='th' scope='row'>
											<Typography>{item.TakedBy}</Typography>
										</TableCell>

										<TableCell component='th' scope='row' align='center'>
											<Typography>
												{getBookingTimeInterval(item.TakeAt, item.ReturnedAt)}
											</Typography>
										</TableCell>
									</TableRow>
								))}
						</TableBody>
					</Table>
				</TableContainer>
			</Box>
		</ModalGeneral>
	);
});
