export interface IErrorResponse {
	Errors: IErrorItem[];
	error: string;
	error_description: string;
}

export interface IErrorItem {
	FieldName: string;
	Message: string;
}
