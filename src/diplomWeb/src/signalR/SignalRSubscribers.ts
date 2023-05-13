import { injectable } from 'inversify';
import { IDashboardInfoResponse } from '../models/interfaces/response/IDashboardResponse';
import EventSubscriber from './EventSubscriber';

@injectable()
export default class SignalRSubscribers {
	dashboardSubscriber = new EventSubscriber<IDashboardInfoResponse>();
}
