import { observer } from 'mobx-react-lite';
import { useEffect } from 'react';
import ModalConfirmStore from '../../store/base/helpers/ModalConfirmStore';
import { ModalGeneral } from './ModalGeneral';

export interface IModalConfirm<T> {
	children?: JSX.Element;
	confirmStore: ModalConfirmStore<T>;
}

export const ModalConfirm = observer(<T,>(props: IModalConfirm<T>): JSX.Element => {
	const { confirmStore } = props;

	const resetData = (): void => {
		confirmStore.errorStore.resetError();
	};

	useEffect(() => {
		return () => {
			resetData();
		};
	}, []);

	return <ModalGeneral modalStore={confirmStore.modalStore}>{props.children}</ModalGeneral>;
});
