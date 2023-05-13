import { injectable } from 'inversify';
import { action, makeObservable } from 'mobx';
import { IEmployee } from '../models/interfaces/IEmployee';
import { IEmployeeInfoResponse } from '../models/interfaces/response/IEmployeeInfoResponse';
import TableDataStore from './base/TableDataStore';

@injectable()
export default class EmployeeStore {
	public tableDataStore = new TableDataStore<IEmployee, IEmployeeInfoResponse>('users');

	constructor() {
		makeObservable(this);
	}

	@action
	public async syncEmployees(): Promise<void> {
		await this.tableDataStore.service.syncTableData();
	}
}
