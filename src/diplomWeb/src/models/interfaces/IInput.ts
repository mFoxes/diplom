import { InputHTMLAttributes } from 'react';
import ErrorStore from '../../store/ErrorStore';

export interface IInput<T> extends InputHTMLAttributes<HTMLInputElement> {
	label?: string;
	inputName: string;
	serverErrorStore?: ErrorStore;
	hasErrorField?: boolean;
}
