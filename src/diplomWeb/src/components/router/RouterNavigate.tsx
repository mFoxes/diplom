import { Navigate } from 'react-router-dom';
import { useInject } from '../../hooks/useInject';
import { Types } from '../../inversify/inversify.types';
import AuthStore from '../../store/AuthStore';

export interface IRouterNavigate {
	element: JSX.Element;
}

export const RouterNavigate = ({ element, ...props }: IRouterNavigate): JSX.Element => {
	const authStore = useInject<AuthStore>(Types.AuthStore);

	return authStore.isAdmin ? element : <Navigate to='/login' />;
};
