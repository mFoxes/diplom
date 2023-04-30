import { errorResponse } from '../models/interfaces/response/errorResponse';
import { FieldError } from 'react-hook-form';

export default class ErrorsHelper {
	static chooseErrors = (
		fieldName: string,
		serverErrors?: errorResponse[],
		formErrors?: FieldError | null,
	): errorResponse[] => {
		let tempErrorsList: errorResponse[] = [];
		if (serverErrors) {
			tempErrorsList = serverErrors;
		}
		if (formErrors && formErrors.message) {
			tempErrorsList = [{ FieldName: fieldName, Message: formErrors.message }];
		}

		return tempErrorsList;
	};

	static getErrorMessagesByName = (
		fullErrorList: errorResponse[] | undefined,
		filedName: string,
	): string[] | undefined => {
		if (fullErrorList) {
			return fullErrorList.filter((item) => item.FieldName === filedName).map((item) => `${item.Message}\n`);
		}
	};
}
