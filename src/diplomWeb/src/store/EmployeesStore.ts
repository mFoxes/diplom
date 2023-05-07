import { action, makeObservable } from 'mobx';
import { IEmployee } from '../models/interfaces/IEmployee';
import { employeeInfoResponse } from '../models/interfaces/response/employeeInfoResponse';
import TableDataStore from './base/TableDataStore';

export default class EmployeeStore extends TableDataStore<IEmployee, employeeInfoResponse> {
	constructor() {
		super('users');
		makeObservable(this);
	}

	@action
	public async syncEmployees(): Promise<void> {
		await this.service.syncTableData();
	}
}
