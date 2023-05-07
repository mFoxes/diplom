import { inject } from 'inversify';
import { action, computed, makeObservable, observable } from 'mobx';
import { Types } from '../inversify/inversify.types';
import IDashboard from '../models/interfaces/IDashboard';
import { IEmployee } from '../models/interfaces/IEmployee';
import { dashboardInfoResponse } from '../models/interfaces/response/dashboardResponse';
import SignalRSubscribers from '../signalR/SignalRSubscribers';
import TableDataStore from './base/TableDataStore';

export default class DashboardStore extends TableDataStore<IDashboard, dashboardInfoResponse> {
	@inject(Types.SignalRSubscribers) private _signalRSubscribers!: SignalRSubscribers;

	constructor() {
		super('bookings');

		this._signalRSubscribers.dashboardSubscriber.Add((data?: dashboardInfoResponse) => {
			this.changeDashboardItem(data);
		});

		makeObservable(this);
	}

	@action
	public changeDashboardItem(data?: dashboardInfoResponse): void {
		const itemIndex = this._items.findIndex((item) => item.Id === data?.Id);
		if (data && itemIndex !== undefined) {
			this.setItemById(itemIndex, data);
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
		const res = await this.service.getAllEmployeeNames();

		if (res.isRight()) {
			this.setEmployees(res.value);
		}
	}
}
