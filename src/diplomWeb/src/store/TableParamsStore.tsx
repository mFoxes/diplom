import { makeAutoObservable } from 'mobx';
import { FilterDirType } from '../components/UI/icons/sort/SortBtn';
import { IFilterVariable, ITableParams } from '../models/interfaces/ITableParams';
import { DEFAULT_PAGE_ITEMS_COUNT } from '../staticData';

export default class TableParamsStore {
	private _skip = 0;
	private _take = DEFAULT_PAGE_ITEMS_COUNT;
	private _orderBy = 'Name';
	private _orderDir: FilterDirType = 'asc';
	private _filter: IFilterVariable = {};

	private _page = 1;

	constructor() {
		makeAutoObservable(this);
	}

	get skip(): number {
		return this._skip;
	}

	public setSkip(data: number): void {
		this._skip = data;
	}

	get take(): number {
		return this._take;
	}

	public setTake(data: number): void {
		this._take = data;
	}

	get orderBy(): string {
		return this._orderBy;
	}

	public setOrderBy(data: string): void {
		this._orderBy = data;
	}

	get orderDir(): FilterDirType {
		return this._orderDir;
	}

	public setOrderDir(data: FilterDirType): void {
		this._orderDir = data;
	}

	get filter(): IFilterVariable {
		return this._filter;
	}

	public filterByKey(key: string): string | number {
		return this._filter[key];
	}

	public setFilter(data: IFilterVariable): void {
		this._filter = data;
	}

	public setFilterByKey(key: string, data: string): void {
		if (this._filter[key] !== data) {
			this._filter[key] = data;
		}
	}

	get page(): number {
		return this._page;
	}

	public setPage(data: number): void {
		this._page = data;
	}

	public handleChange(value: number): void {
		this._page = value;
		this._skip = (value - 1) * this._take;
	}

	public getTableParams(): ITableParams {
		const params: ITableParams = {
			skip: this._skip,
			take: this._take,
			orderBy: this._orderBy,
			orderDir: this._orderDir,
		};
		Object.keys(this.filter).forEach((item) => {
			params[`filter_${item}`] = this._filter[item];
		});

		return params;
	}

	public resetTableParams(): void {
		this._skip = 0;
		this._take = DEFAULT_PAGE_ITEMS_COUNT;
		this._orderBy = 'Name';
		this._orderDir = 'asc';
		Object.keys(this.filter).forEach((item) => {
			delete this._filter[item];
		});
		this._page = 0;
	}

	public changeSortOrderByFieldName(fieldName: string): void {
		if (this.orderBy === fieldName) {
			this._orderDir === 'asc' ? this.setOrderDir('desc') : this.setOrderDir('asc');
		} else {
			this.setOrderBy(fieldName);
			this.setOrderDir('asc');
		}
	}
}
