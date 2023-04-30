import { useContext } from 'react';
import { Navigate } from 'react-router-dom';
import { Context } from '../..';

export interface IRouterNavigate {
	element: JSX.Element;
}

export const RouterNavigate = ({ element, ...props }: IRouterNavigate): JSX.Element => {
	const { authStore } = useContext(Context);

	return authStore.isAdmin ? element : <Navigate to='/login' />;
};
