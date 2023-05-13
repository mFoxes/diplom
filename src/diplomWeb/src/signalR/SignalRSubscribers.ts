import { IDashboardInfoResponse } from '../models/interfaces/response/IDashboardResponse';
import EventSubscriber from './EventSubscriber';

export default class SignalRSubscribers {
	dashboardSubscriber = new EventSubscriber<IDashboardInfoResponse>();
}
