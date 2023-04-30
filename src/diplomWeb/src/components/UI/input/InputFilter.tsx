import { TextField } from '@mui/material';
import { observer } from 'mobx-react-lite';
import React, { useCallback } from 'react';
import { debounce } from 'throttle-debounce';
import { IData } from '../../../models/interfaces/IData';
import { IInput } from '../../../models/interfaces/IInput';
import { dataInfoResponse } from '../../../models/interfaces/response/dataInfoResponse';
import TableDataStore from '../../../store/TableDataStore';
import { InputCleanButton } from '../icons/clean/InputCleanButton';

interface IFilterInput<IItem extends IData, IInfoResponse extends dataInfoResponse> extends IInput<IItem> {
	store: TableDataStore<IItem, IInfoResponse>;
}

export const InputFilter = observer(
	<IItem extends IData, IInfoResponse extends dataInfoResponse>({
		inputName,
		store,
		...props
	}: IFilterInput<IItem, IInfoResponse>): JSX.Element => {
		const debounceTime = 500;

		const updateTableDataDebounce = useCallback(
			debounce(debounceTime, (): void => store.updateTableData()),
			[],
		);

		const handleFieldChange = (value: string): void => {
			store.params.setFilterByKey(inputName, value);
			updateTableDataDebounce();
		};

		const handleInputChange = (e: React.ChangeEvent<HTMLInputElement>): void => {
			handleFieldChange(e.target.value);
		};

		const handleCleanField = (): void => {
			handleFieldChange('');
		};

		return (
			<TextField
				inputProps={{ ...props }}
				value={store.params.filter[inputName] || ''}
				onChange={handleInputChange}
				InputProps={
					store.params.filter[inputName] && store.params.filter[inputName] !== ''
						? { endAdornment: <InputCleanButton handleCleanField={handleCleanField} /> }
						: {}
				}
				variant='standard'
				sx={{ width: '100%' }}
			/>
		);
	},
);
