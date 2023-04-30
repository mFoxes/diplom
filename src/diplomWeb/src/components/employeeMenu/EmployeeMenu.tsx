import { Menu, MenuItem, Typography } from '@mui/material';
import { observer } from 'mobx-react-lite';
import { RefObject, useContext } from 'react';
import { Context } from '../..';

interface IEmployeeMenu {
	anchorEl: RefObject<HTMLButtonElement> | null;
}

export const EmployeeMenu = observer(({ anchorEl, ...props }: IEmployeeMenu): JSX.Element => {
	const { authStore, employeesStore } = useContext(Context);

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
						employeesStore.modalInfo.setTableDataInfoId(authStore.currentEmployee?.Id);
						employeesStore.modalInfo.setTableDataInfo(authStore.currentEmployee);
						employeesStore.modalInfo.modalStore.handleOpen();
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
