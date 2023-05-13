import { makeAutoObservable } from 'mobx';
import { IErrorResponse } from '../../../models/interfaces/response/IErrorResponse';

export default class ErrorStore {
	private _error: IErrorResponse[] | undefined;

	constructor() {
		makeAutoObservable(this);
	}

	get error(): IErrorResponse[] | undefined {
		return this._error;
	}

	public setError(error: IErrorResponse[] | undefined): void {
		this._error = error;
	}

	public resetError(): void {
		this.setError(undefined);
	}

	public removeErrorByName(inputName: string): void {
		this.setError(this.error?.filter((item) => item.FieldName !== inputName));
	}
}
