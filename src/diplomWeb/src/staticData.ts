export const API_URL = '/api';
export const CLIENT_ID = 'grandmaSpaClient';

export const MIN_NUM = 1;
export const MAX_NUM = 80;

export const REQUIRED_PHOTO_ERROR = 'Поле полжно содержать фотографию';
export const REQUIRED_TEXT_ERROR = 'Поле ввода не должно быть пустым';
export const MIN_TEXT_ERROR = 'Поле ввода не должно быть пустым';
export const MAX_TEXT_ERROR = (value: number): string => `Ввод не должен превышать ${value} символов`;
export const SPACE_TEXT_ERROR = 'Ввод не должен содержать пробелы';

export const DRAWER_WIDTH = 300;
export const DRAWER_HEIGHT = 80;

export const DEFAULT_PAGE_ITEMS_COUNT = 15;

export const QR_CODE_VALUE_GENERATOR = (qrCodeValue: string): string =>
	`http://glpi.singularis-lab.com/front/peripheral.php?is_deleted=0&as_map=0&criteria%5B0%5D%5Blink%5D=AND&criteria%5B0%5D%5Bfield%5D=6&criteria%5B0%5D%5Bsearchtype%5D=contains&criteria%5B0%5D%5Bvalue%5D=${qrCodeValue}&search=Search&itemtype=Peripheral&start=0`;
