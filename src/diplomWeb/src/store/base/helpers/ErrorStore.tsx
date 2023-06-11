import { makeAutoObservable } from 'mobx';
import { IErrorItem } from '../../../models/interfaces/response/IErrorResponse';

export default class ErrorStore {
	private _error: IErrorItem[] | undefined;

	constructor() {
		makeAutoObservable(this);
	}

	get error(): IErrorItem[] | undefined {
		return this._error;
	}

	public setError(error: IErrorItem[] | undefined): void {
		this._error = error;
	}

	public resetError(): void {
		this.setError(undefined);
	}

	public removeErrorByName(inputName: string): void {
		this.setError(this.error?.filter((item) => item.FieldName !== inputName));
	}
}
