import { inject, injectable } from 'inversify';
import { action, computed, makeObservable, observable } from 'mobx';
import { Types } from '../inversify/inversify.types';
import IDashboard from '../models/interfaces/IDashboard';
import { IEmployee } from '../models/interfaces/IEmployee';
import { IDashboardInfoResponse } from '../models/interfaces/response/IDashboardResponse';
import SignalRSubscribers from '../signalR/SignalRSubscribers';
import TableDataStore from './base/TableDataStore';

@injectable()
export default class DashboardStore extends TableDataStore<IDashboard, IDashboardInfoResponse> {
	@inject(Types.SignalRSubscribers) private _signalRSubscribers!: SignalRSubscribers;

	constructor(@inject(Types.DashboardRequestAddress) requestAddress: string) {
		super(requestAddress);

		this._signalRSubscribers.dashboardSubscriber.Add((data?: IDashboardInfoResponse) => {
			this.changeDashboardItem(data);
		});

		makeObservable(this);
	}

	@action
	public changeDashboardItem(data?: IDashboardInfoResponse): void {
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
