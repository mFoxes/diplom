import { Menu, MenuItem, Typography } from '@mui/material';
import { observer } from 'mobx-react-lite';
import { RefObject } from 'react';
import { useInject } from '../../hooks/useInject';
import { Types } from '../../inversify/inversify.types';
import AuthStore from '../../store/AuthStore';
import EmployeeStore from '../../store/EmployeesStore';

interface IEmployeeMenu {
	anchorEl: RefObject<HTMLButtonElement> | null;
}

export const EmployeeMenu = observer(({ anchorEl, ...props }: IEmployeeMenu): JSX.Element => {
	const authStore = useInject<AuthStore>(Types.AuthStore);
	const employeesStore = useInject<EmployeeStore>(Types.EmployeeStore);
	const { tableDataStore: employeesTableStore } = employeesStore;

	return (
		<Menu
			sx={{ mt: '45px' }}
			id='menu-appbar'
			anchorEl={anchorEl?.current}
			anchorOrigin={{
				vertical: 'top',
				horizontal: 'right',
			}}
			keepMounted
			transformOrigin={{
				vertical: 'top',
				horizontal: 'right',
			}}
			open={authStore.employeeMenuActive}
			onClose={(): void => {
				authStore.handleMenuClose();
			}}
		>
			<MenuItem
				key={'edit'}
				onClick={(): void => {
					if (authStore.currentEmployee?.Id) {
						employeesTableStore.modalInfo.setTableDataInfoId(authStore.currentEmployee?.Id);
						employeesTableStore.modalInfo.setTableDataInfo(authStore.currentEmployee);
						employeesTableStore.modalInfo.modalStore.handleOpen();
					}
					authStore.handleMenuClose();
				}}
			>
				<Typography textAlign='center'>Редактировать профиль</Typography>
			</MenuItem>

			<MenuItem
				key={'exit'}
				onClick={(): void => {
					authStore.modalConfirm.modalStore.handleOpen();
					authStore.handleMenuClose();
				}}
			>
				<Typography textAlign='center'>Выход</Typography>
			</MenuItem>
		</Menu>
	);
});
