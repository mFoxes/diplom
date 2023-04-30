import { FilterDirType } from '../../components/UI/icons/sort/SortBtn';

export interface ITableParams extends IFilterVariable {
	skip: number;
	take: number;
	orderBy: string;
	orderDir: FilterDirType;
}

export interface IFilterVariable {
	[key: string]: string | number;
}
