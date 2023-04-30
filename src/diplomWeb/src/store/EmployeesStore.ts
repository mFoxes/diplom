import { action, makeObservable } from 'mobx';
import { IEmployee } from '../models/interfaces/IEmployee';
import { employeeInfoResponse } from '../models/interfaces/response/employeeInfoResponse';
import TableDataStore from './TableDataStore';

export default class EmployeeStore extends TableDataStore<IEmployee, employeeInfoResponse> {
	constructor(requestAddress: string) {
		super(requestAddress);
		makeObservable(this);
	}

	@action
	public async syncEmployees(): Promise<void> {
		await this.service.syncTableData();
	}
}
