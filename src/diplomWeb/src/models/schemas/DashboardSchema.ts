import * as yup from 'yup';
import { MAX_TEXT_ERROR, MIN_NUM, MAX_NUM, MIN_TEXT_ERROR, REQUIRED_TEXT_ERROR } from '../../staticData';

export const DashboardInfoSchema = yup
	.object({
		UserId: yup.string().min(MIN_NUM, MIN_TEXT_ERROR).max(MAX_NUM, MAX_TEXT_ERROR(MAX_NUM)),
		TakeAt: yup.date().required(REQUIRED_TEXT_ERROR),
		ReturnAt: yup.date().required(REQUIRED_TEXT_ERROR),
	})
	.required();
