import html2canvas from 'html2canvas';
import { renderToStaticMarkup } from 'react-dom/server';
import { FieldError } from 'react-hook-form';
import { QrCodeImg } from '../components/qrCodeImg/QrCodeImg';
import { IErrorItem } from '../models/interfaces/response/IErrorResponse';

import { format } from 'date-fns';
import download from 'downloadjs';
import ErrorsHelper from '../helpers/ErrorsHelpers';

export const nameof = <T,>(name: keyof T): keyof T => name;

export const getErrorListByName = (
	inputName: string,
	serverErrorList: IErrorItem[] | undefined,
	formErrorList?: FieldError,
): string[] | undefined => {
	const fullErrorList = ErrorsHelper.chooseErrors(inputName, serverErrorList, formErrorList);

	const errorListByInputName = ErrorsHelper.getErrorMessagesByName(fullErrorList, inputName);
	return errorListByInputName;
};

export const createQrCode = async (qrCodeValue: string): Promise<void> => {
	const output = document.createElement('div');
	const element = <QrCodeImg qrCodeValue={qrCodeValue} />;
	const staticElement = renderToStaticMarkup(element);
	output.innerHTML = `<div>${staticElement}</div>`;

	document.body.appendChild(output);
	const htmlElement = document.getElementById(`qr-code-${qrCodeValue}`);

	if (htmlElement) {
		await createAndDownloadPng(htmlElement, qrCodeValue);
	}

	document.body.removeChild(output);
};

const createAndDownloadPng = async (htmlElement: HTMLElement, qrCodeValue: string): Promise<void> => {
	const canvas = await html2canvas(htmlElement);
	const pngUrl = canvas.toDataURL('image/png').replace('image/png', 'image/octet-stream');
	download(pngUrl, `${qrCodeValue}.png`);
};

export const getBookingTimeInterval = (TakeAt: Date | undefined, ReturnAt: Date | undefined): JSX.Element => {
	let takeAt = '';
	let returnAt = '';

	if (TakeAt && ReturnAt) {
		takeAt = format(new Date(TakeAt), 'dd.MM.yyyy');
		returnAt = format(new Date(ReturnAt), 'dd.MM.yyyy');
	}

	return (
		<>
			{takeAt} - {returnAt}
		</>
	);
};
