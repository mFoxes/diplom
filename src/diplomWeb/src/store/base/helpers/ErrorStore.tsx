import { makeAutoObservable } from 'mobx';
import { errorResponse } from '../../../models/interfaces/response/errorResponse';

export default class ErrorStore {
	private _error: errorResponse[] | undefined;

	constructor() {
		makeAutoObservable(this);
	}

	get error(): errorResponse[] | undefined {
		return this._error;
	}

	public setError(error: errorResponse[] | undefined): void {
		this._error = error;
	}

	public resetError(): void {
		this.setError(undefined);
	}

	public removeErrorByName(inputName: string): void {
		this.setError(this.error?.filter((item) => item.FieldName !== inputName));
	}
}
