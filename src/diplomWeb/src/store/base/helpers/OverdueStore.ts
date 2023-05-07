import { makeAutoObservable } from 'mobx';
import IDashboard from '../../../models/interfaces/IDashboard';
import ModalStore from '../ModalStore';

export default class OverdueStore {
	private _currentEmployeeOverdue: IDashboard[] | undefined;
	private _currentEmployeeOverdueCount: number | undefined;

	public readonly modalStore = new ModalStore();

	constructor() {
		makeAutoObservable(this);
	}

	get currentEmployeeOverdueCount(): number | undefined {
		return this._currentEmployeeOverdueCount;
	}

	public setCurrentEmployeeOverdueCount(data: number | undefined): void {
		this._currentEmployeeOverdueCount = data;
	}

	get currentEmployeeOverdue(): IDashboard[] | undefined {
		return this._currentEmployeeOverdue;
	}

	public setCurrentEmployeeOverdue(data: IDashboard[] | undefined): void {
		this._currentEmployeeOverdue = data;
	}
}
