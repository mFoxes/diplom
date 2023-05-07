import { Container } from 'inversify';
import 'reflect-metadata';
import AuthService from '../service/api/authService';
import CurrentEmployeeService from '../service/api/currentEmployeeService';
import DownloadableImageService from '../service/api/downloadableImgService';
import { Types } from './inversify.types';
import SignalR from '../signalR/SignalR';
import SignalRSubscribers from '../signalR/SignalRSubscribers';
import AuthStore from '../store/AuthStore';
import DashboardStore from '../store/DashboardStore';
import EmployeeStore from '../store/EmployeesStore';
import GeneralStore from '../store/GeneralStore';
import DevicesStore from '../store/DevicesStore';
import PhotosCacheStore from '../store/base/helpers/PhotosCacheStore';
import LocalStorageService from '../service/localStorageService';

const container = new Container();

// service
container.bind<AuthService>(Types.AuthService).to(AuthService);
container.bind<LocalStorageService>(Types.LocalStorageService).to(LocalStorageService);
container.bind<CurrentEmployeeService>(Types.CurrentEmployeeService).to(CurrentEmployeeService);
container.bind<DownloadableImageService>(Types.DownloadableImageService).to(DownloadableImageService);
// service end

// stores
container.bind<AuthStore>(Types.AuthStore).to(AuthStore).inSingletonScope();
container.bind<DashboardStore>(Types.DashboardStore).to(DashboardStore).inSingletonScope();
container.bind<DevicesStore>(Types.DevicesStore).to(DevicesStore).inSingletonScope();
container.bind<EmployeeStore>(Types.EmployeeStore).to(EmployeeStore).inSingletonScope();
container.bind<GeneralStore>(Types.GeneralStore).to(GeneralStore).inSingletonScope();

container.bind<PhotosCacheStore>(Types.PhotosCacheStore).to(PhotosCacheStore).inSingletonScope();
// stores end

// signalR
container.bind<SignalR>(Types.SignalR).to(SignalR).inSingletonScope();
container.bind<SignalRSubscribers>(Types.SignalRSubscribers).to(SignalRSubscribers).inSingletonScope();
// signalR end

export default container;
