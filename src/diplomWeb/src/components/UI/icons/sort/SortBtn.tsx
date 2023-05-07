import { ArrowDownward } from '@mui/icons-material';
import { IconButton } from '@mui/material';
import { observer } from 'mobx-react-lite';
import TableParamsStore from '../../../../store/base/helpers/TableParamsStore';

export type FilterDirType = 'asc' | 'desc';

export interface ISortBtn {
	fieldName: string;
	paramsStore: TableParamsStore;
}

export const SortBtn = observer(({ fieldName, paramsStore, ...params }: ISortBtn): JSX.Element => {
	return (
		<IconButton
			onClick={(): void => {
				paramsStore.changeSortOrderByFieldName(fieldName);
			}}
		>
			<ArrowDownward
				color={fieldName === paramsStore.orderBy ? 'primary' : 'inherit'}
				sx={[
					paramsStore.orderDir === 'asc' && fieldName === paramsStore.orderBy
						? {}
						: { transform: 'scale(1,-1)' },
				]}
			/>
		</IconButton>
	);
});
