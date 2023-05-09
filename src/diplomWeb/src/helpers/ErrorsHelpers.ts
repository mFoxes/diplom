import { IErrorResponse } from '../models/interfaces/response/IErrorResponse';
import { FieldError } from 'react-hook-form';

export default class ErrorsHelper {
	static chooseErrors = (
		fieldName: string,
		serverErrors?: IErrorResponse[],
		formErrors?: FieldError | null,
	): IErrorResponse[] => {
		let tempErrorsList: IErrorResponse[] = [];
		if (serverErrors) {
			tempErrorsList = serverErrors;
		}
		if (formErrors && formErrors.message) {
			tempErrorsList = [{ FieldName: fieldName, Message: formErrors.message }];
		}

		return tempErrorsList;
	};

	static getErrorMessagesByName = (
		fullErrorList: IErrorResponse[] | undefined,
		filedName: string,
	): string[] | undefined => {
		if (fullErrorList) {
			return fullErrorList.filter((item) => item.FieldName === filedName).map((item) => `${item.Message}\n`);
		}
	};
}
