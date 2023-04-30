import { Container } from 'inversify';
import 'reflect-metadata';
import AuthService from '../service/api/authService';
import { Types } from './inversify.types';
import LocalStorageService from '../service/LocalStorageService';

const container = new Container();

// service
container.bind<AuthService>(Types.AuthService).to(AuthService);
container.bind<LocalStorageService>(Types.LocalStorageService).to(LocalStorageService);
// service end

// stores
// stores end

// signalR
// signalR end

export default container;
