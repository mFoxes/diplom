import { Pagination } from '@mui/material';
import { observer } from 'mobx-react-lite';
import React from 'react';
import { IData } from '../../../models/interfaces/IData';
import { IDataInfoResponse } from '../../../models/interfaces/response/IDataInfoResponse';
import TableDataStore from '../../../store/base/TableDataStore';

export interface IListPagination<IItem extends IData, IInfoResponse extends IDataInfoResponse> {
	store: TableDataStore<IItem, IInfoResponse>;
}

export const ListPagination = observer(
	<IItem extends IData, IInfoResponse extends IDataInfoResponse>({
		store,
		...props
	}: IListPagination<IItem, IInfoResponse>): JSX.Element => {
		return store.paginationLength > 1 ? (
			<Pagination
				{...props}
				count={store.paginationLength}
				onChange={(event: React.ChangeEvent<unknown>, value: number): void => store.params.handleChange(value)}
			/>
		) : (
			<></>
		);
	},
);
