import { inject, injectable } from 'inversify';
import { action, computed, makeObservable, observable } from 'mobx';
import { Types } from '../inversify/inversify.types';
import IDashboard from '../models/interfaces/IDashboard';
import { IEmployee } from '../models/interfaces/IEmployee';
import { IDashboardInfoResponse } from '../models/interfaces/response/IDashboardResponse';
import SignalRSubscribers from '../signalR/SignalRSubscribers';
import TableDataStore from './base/TableDataStore';

@injectable()
export default class DashboardStore {
	@inject(Types.SignalRSubscribers) private _signalRSubscribers!: SignalRSubscribers;

	public tableDataStore = new TableDataStore<IDashboard, IDashboardInfoResponse>('bookings');

	constructor() {
		makeObservable(this);
	}

	@action
	public initSignalRSubscription(): void {
		this._signalRSubscribers.dashboardSubscriber.Add((data?: IDashboardInfoResponse) => {
			this.changeDashboardItem(data);
		});
	}

	@action
	public changeDashboardItem(data?: IDashboardInfoResponse): void {
		const itemIndex = this.tableDataStore.items.findIndex((item) => item.Id === data?.Id);
		if (data && itemIndex !== undefined) {
			this.tableDataStore.setItemById(itemIndex, data);
		}
	}

	@observable private _employees: IEmployee[] = [];

	@computed
	get employees(): IEmployee[] {
		return this._employees;
	}

	@action
	public setEmployees(data: IEmployee[]): void {
		this._employees = data;
	}

	@action
	public async getAllEmployeeNames(): Promise<void> {
		const res = await this.tableDataStore.service.getAllEmployeeNames();

		if (res.isRight()) {
			this.setEmployees(res.value);
		}
	}
}
