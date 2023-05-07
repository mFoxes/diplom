import { Button } from '@mui/material';
import { observer } from 'mobx-react-lite';
import ModalConfirmStore from '../../../store/base/helpers/ModalConfirmStore';
import { SecondThemeWrapper } from '../../themeWrapper/SecondThemeWrapper';

export interface ICancelButton<IInfoResponse> {
	modalConfirm: ModalConfirmStore<IInfoResponse>;
	children?: JSX.Element;
}

export const CancelButton = observer(
	<IInfoResponse,>({ modalConfirm, children, ...props }: ICancelButton<IInfoResponse>): JSX.Element => {
		return (
			<SecondThemeWrapper>
				<Button onClick={(): void => modalConfirm.modalStore.handleClose()} variant='contained'>
					{children}
				</Button>
			</SecondThemeWrapper>
		);
	},
);
