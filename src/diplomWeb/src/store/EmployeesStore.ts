import { action, makeObservable } from 'mobx';
import { IEmployee } from '../models/interfaces/IEmployee';
import { IEmployeeInfoResponse } from '../models/interfaces/response/IEmployeeInfoResponse';
import TableDataStore from './base/TableDataStore';
import { inject, injectable } from 'inversify';
import { Types } from '../inversify/inversify.types';

@injectable()
export default class EmployeeStore extends TableDataStore<IEmployee, IEmployeeInfoResponse> {
	constructor(@inject(Types.EmployeeRequestAddress) requestAddress: string) {
		super(requestAddress);
		makeObservable(this);
	}

	@action
	public async syncEmployees(): Promise<void> {
		await this.service.syncTableData();
	}
}
