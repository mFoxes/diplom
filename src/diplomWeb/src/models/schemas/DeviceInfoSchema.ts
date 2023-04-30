import * as yup from 'yup';
import { MAX_TEXT_ERROR, MIN_NUM, MIN_TEXT_ERROR, REQUIRED_TEXT_ERROR } from '../../staticData';

const maxNum = 255;

export const DeviceInfoSchema = yup
	.object({
		Name: yup
			.string()
			.required(REQUIRED_TEXT_ERROR)
			.min(MIN_NUM, MIN_TEXT_ERROR)
			.max(maxNum, MAX_TEXT_ERROR(maxNum)),
		InventoryNumber: yup
			.string()
			.required(REQUIRED_TEXT_ERROR)
			.min(MIN_NUM, MIN_TEXT_ERROR)
			.max(maxNum, MAX_TEXT_ERROR(maxNum)),
		Photo: yup.mixed(),
	})
	.required();
