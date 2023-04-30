import * as yup from 'yup';
import {
	MAX_TEXT_ERROR,
	MIN_NUM,
	MAX_NUM,
	MIN_TEXT_ERROR,
	REQUIRED_TEXT_ERROR,
	SPACE_TEXT_ERROR,
} from '../../staticData';

export const LoginSchema = yup
	.object({
		Login: yup
			.string()
			.required(REQUIRED_TEXT_ERROR)
			.min(MIN_NUM, MIN_TEXT_ERROR)
			.max(MAX_NUM, MAX_TEXT_ERROR(MAX_NUM))
			.matches(/[^\s]/, SPACE_TEXT_ERROR),
		Password: yup
			.string()
			.required(REQUIRED_TEXT_ERROR)
			.max(MAX_NUM, MAX_TEXT_ERROR(MAX_NUM))
			.matches(/[^\s]/, SPACE_TEXT_ERROR),
		remember: yup.bool(),
	})
	.required();
