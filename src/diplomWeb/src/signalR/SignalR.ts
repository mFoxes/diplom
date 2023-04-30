import { HttpTransportType, HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { dashboardInfoResponse } from '../models/interfaces/response/dashboardResponse';
import { API_URL } from '../staticData';
import SignalRSubscribers from './SignalRSubscribers';

export default class SignalR {
	private _signalRSubscribers: SignalRSubscribers | undefined;
	private _connection: HubConnection | undefined;

	constructor(signalRSubscribers: SignalRSubscribers) {
		this._signalRSubscribers = signalRSubscribers;

		this.initHub();
		this.addFunctionListener();
	}

	private async initHub(): Promise<void> {
		this._connection = new HubConnectionBuilder()
			.withUrl(`${API_URL}/hubs/booking`, {
				skipNegotiation: true,
				transport: HttpTransportType.WebSockets,
			})
			.withAutomaticReconnect()
			.build();

		this._connection.start();
	}

	private addFunctionListener(): void {
		if (this._connection) {
			this._connection.on('Notify', (device: dashboardInfoResponse) => {
				this._signalRSubscribers?.dashboardSubscriber.TriggerEvent(device);
			});
		}
	}
}
