import { Typography } from '@mui/material';
import { observer } from 'mobx-react-lite';
import TableParamsStore from '../../store/base/helpers/TableParamsStore';
import { ISortBtn, SortBtn } from '../UI/icons/sort/SortBtn';

export interface ITableHeaderCell extends ISortBtn {
	text: string;
	paramsStore: TableParamsStore;
}

export const TableHeadCell = observer((params: ITableHeaderCell): JSX.Element => {
	return (
		<>
			<Typography component='span'>{params.text}</Typography>

			<SortBtn fieldName={params.fieldName} paramsStore={params.paramsStore} />
		</>
	);
});
