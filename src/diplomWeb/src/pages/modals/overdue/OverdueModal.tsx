import { Close } from '@mui/icons-material';
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
import { ModalGeneral } from '../../../components/modal/ModalGeneral';
import { useInject } from '../../../hooks/useInject';
import { Types } from '../../../inversify/inversify.types';
import AuthStore from '../../../store/AuthStore';
import { getBookingTimeInterval } from '../../../utilities/Utilities';

export const OverdueModal = observer((): JSX.Element => {
	const authStore = useInject<AuthStore>(Types.AuthStore);

	useEffect(() => {
		authStore.getCurrentEmployeeOverdue();
	}, []);

	return (
		<ModalGeneral modalStore={authStore.overdueStore.modalStore}>
			<Box sx={{ paddingTop: '40px' }}>
				<IconButton
					onClick={(): void => {
						authStore.overdueStore.modalStore.handleClose();
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
								<TableCell sx={{ fontSize: '16px' }}>Название</TableCell>
								<TableCell sx={{ fontSize: '16px' }}>Инвентарный номер</TableCell>
								<TableCell sx={{ fontSize: '16px' }} align='center'>
									Период
								</TableCell>
							</TableRow>
						</TableHead>
						<TableBody>
							{authStore.overdueStore.currentEmployeeOverdue &&
								authStore.overdueStore.currentEmployeeOverdue.map((item, indx) => (
									<TableRow
										key={item.InventoryNumber + indx}
										sx={{ '&:last-child td, &:last-child th': { border: 0 }, maxHeight: 20 }}
									>
										<TableCell component='th' scope='row'>
											<Typography>{item.Name}</Typography>
										</TableCell>

										<TableCell component='th' scope='row'>
											<Typography>{item.InventoryNumber}</Typography>
										</TableCell>

										<TableCell component='th' scope='row' align='center'>
											<Typography>
												{getBookingTimeInterval(item.TakeAt, item.ReturnAt)}
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
