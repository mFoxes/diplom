export interface IPageDataResponse<TItem> {
	TotalItems: number;
	TotalItemsFiltered: number;
	Items: TItem[];
}
