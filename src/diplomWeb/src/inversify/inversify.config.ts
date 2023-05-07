import { Container } from 'inversify';
import 'reflect-metadata';
import AuthService from '../service/api/authService';
import CurrentEmployeeService from '../service/api/currentEmployeeService';
import DownloadableImageService from '../service/api/downloadableImgService';
import LocalStorageService from '../service/localStorageService';
import { Types } from './inversify.types';

const container = new Container();

// service
container.bind<AuthService>(Types.AuthService).to(AuthService);
container.bind<LocalStorageService>(Types.LocalStorageService).to(LocalStorageService);
container.bind<CurrentEmployeeService>(Types.CurrentEmployeeService).to(CurrentEmployeeService);
container.bind<DownloadableImageService>(Types.DownloadableImageService).to(DownloadableImageService);
// service end

// stores
// stores end

// signalR
// signalR end

export default container;
