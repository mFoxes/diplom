import { SxProps, TableCell, Theme, Typography } from '@mui/material';
import { observer } from 'mobx-react-lite';
import { Types } from '../../inversify/inversify.types';
import IDashboard from '../../models/interfaces/IDashboard';
import GeneralStore from '../../store/GeneralStore';
import { getBookingTimeInterval } from '../../utilities/Utilities';
import { useInject } from '../../hooks/useInject';

interface IDashboardStatus {
	deviceItem: IDashboard;
}

export const DashboardStatus = observer((props: IDashboardStatus): JSX.Element => {
	const generalStore = useInject<GeneralStore>(Types.GeneralStore);

	const checkOverdue = (returnAt: Date): boolean => {
		returnAt.setHours(23, 59, 59);
		return returnAt <= new Date();
	};

	const getStatusColor = (): SxProps<Theme> | undefined => {
		let color: SxProps<Theme>;

		const isLightTheme = generalStore.themeMode === 'light';

		if (props.deviceItem.State === 'free') {
			color = isLightTheme ? { background: '#EBF9F1' } : { background: '#1a5633' };
		} else if (checkOverdue(new Date(props.deviceItem.ReturnAt))) {
			color = isLightTheme ? { background: '#FBE7E8' } : { background: '#93292f' };
		} else {
			color = isLightTheme ? { background: '#FEF2E5' } : { background: '#b07434' };
		}

		return color;
	};

	return (
		<TableCell component='th' scope='row' align='center' sx={getStatusColor()} width={'35%'}>
			{props.deviceItem.State === 'booked' ? (
				<>
					<Typography
						sx={{
							lineClamp: '2',
							overflow: 'hidden',
							textOverflow: 'ellipsis',
						}}
					>
						{props.deviceItem.TakedBy}
					</Typography>
					<Typography>
						{getBookingTimeInterval(props.deviceItem.TakeAt, props.deviceItem.ReturnAt)}
					</Typography>
				</>
			) : (
				''
			)}
		</TableCell>
	);
});
