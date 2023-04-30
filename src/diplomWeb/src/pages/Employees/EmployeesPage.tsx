import { Edit } from '@mui/icons-material';
import { Box, Button, Card, CardContent, Grid, IconButton, Typography } from '@mui/material';
import { observer } from 'mobx-react-lite';
import { useContext, useEffect } from 'react';
import { Context } from '../..';
import { SortBtn } from '../../components/UI/icons/sort/SortBtn';
import { DownloadableImage } from '../../components/UI/img/DownloadableImage';
import { InputFilter } from '../../components/UI/input/InputFilter';
import { ListPagination } from '../../components/UI/pagination/ListPagination';
import employeePhotoEmpty from '../../img/employeePhotoEmpty.png';
import { IEmployee } from '../../models/interfaces/IEmployee';
import { IErrorType } from '../../models/interfaces/IErrorType';
import { nameof } from '../../utilities/Utilities';
import EmployeeInfo from '../modals/employeeInfo/EmployeeInfo';

const EmployeesPage = (): JSX.Element => {
	const { generalStore, employeesStore } = useContext(Context);

	const { params, modalInfo } = employeesStore;

	const { skip, take, orderBy, orderDir } = params;

	const syncBtn = async (): Promise<void> => {
		try {
			await employeesStore.syncEmployees();
			employeesStore.updateTableData();
		} catch (e: IErrorType) {
			console.log(e);
		}
	};

	useEffect(() => {
		generalStore.setPageTitle('Пользователи');
		params.resetTableParams();
		params.setTake(12);
	}, []);

	useEffect((): void => {
		employeesStore.updateTableData();
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
					gap: '30px',
				}}
			>
				<Box
					sx={{
						display: 'flex',
						alignItems: 'center',
						justifyContent: 'space-between',
						width: '100%',
						gap: 5,
						'@media (max-width:768px)': {
							flexDirection: 'column',
						},
					}}
				>
					<Box>
						<Button onClick={syncBtn}>Синхронизировать</Button>
					</Box>
					<Box sx={{ display: 'flex', alignItems: 'center', gap: 5, maxWidth: '500px', width: '100%' }}>
						<SortBtn fieldName={nameof<IEmployee>('Name')} paramsStore={params} />

						<InputFilter
							inputName={nameof<IEmployee>('Name')}
							placeholder={'Поиск по названию'}
							store={employeesStore}
						/>
					</Box>
				</Box>
				<Grid container spacing={{ xs: 2 }} maxWidth={{ sm: '600px', md: '900px', xl: '1200px' }}>
					{employeesStore.items &&
						employeesStore.items.map((item) => (
							<Grid
								item
								key={item.Id}
								xs={12}
								sm={6}
								md={4}
								xl={3}
								sx={{ display: 'flex', justifyContent: 'center' }}
							>
								<Card key={item.Id} sx={{ width: '200px' }}>
									<DownloadableImage
										photoId={item.PhotoId}
										emptyPhoto={employeePhotoEmpty}
										style={{ height: '200px', width: '200px', objectFit: 'cover' }}
										alt='employee'
									/>
									<CardContent>
										<Box
											sx={{
												display: 'flex',
												alignItems: 'center',
												justifyContent: 'space-between',
											}}
										>
											<Typography
												gutterBottom
												fontSize={'13px'}
												sx={{
													lineClamp: '2',
													overflow: 'hidden',
													textOverflow: 'ellipsis',
												}}
												maxWidth={'calc(100% - 40px)'}
												maxHeight={'60px'}
												component='div'
											>
												{item.Name}
											</Typography>
											<IconButton
												onClick={(): void => {
													employeesStore.modalInfo.modalStore.handleOpen();
													employeesStore.modalInfo.setTableDataInfoId(item.Id);
												}}
											>
												<Edit />
											</IconButton>
										</Box>
									</CardContent>
								</Card>
							</Grid>
						))}
				</Grid>
			</Box>
			<ListPagination store={employeesStore} />
		</Box>
	);
};

export default observer(EmployeesPage);
