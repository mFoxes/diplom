import { dashboardInfoResponse } from '../models/interfaces/response/dashboardResponse';
import EventSubscriber from './EventSubscriber';

export default class SignalRSubscribers {
	dashboardSubscriber = new EventSubscriber<dashboardInfoResponse>();
}
