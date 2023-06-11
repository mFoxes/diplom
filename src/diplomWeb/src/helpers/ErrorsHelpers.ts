import { FieldError } from 'react-hook-form';
import { IErrorItem } from '../models/interfaces/response/IErrorResponse';

export default class ErrorsHelper {
	static chooseErrors = (
		fieldName: string,
		serverErrors?: IErrorItem[],
		formErrors?: FieldError | null,
	): IErrorItem[] => {
		let tempErrorsList: IErrorItem[] = [];
		if (serverErrors) {
			tempErrorsList = serverErrors;
		}
		if (formErrors && formErrors.message) {
			tempErrorsList = [{ FieldName: fieldName, Message: formErrors.message }];
		}

		return tempErrorsList;
	};

	static getErrorMessagesByName = (
		fullErrorList: IErrorItem[] | undefined,
		filedName: string,
	): string[] | undefined => {
		if (fullErrorList) {
			return fullErrorList.filter((item) => item.FieldName === filedName).map((item) => `${item.Message}\n`);
		}
	};
}
