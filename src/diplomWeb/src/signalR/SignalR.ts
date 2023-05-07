import { HttpTransportType, HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { dashboardInfoResponse } from '../models/interfaces/response/dashboardResponse';
import { API_URL } from '../staticData';
import SignalRSubscribers from './SignalRSubscribers';
import { inject } from 'inversify';
import { Types } from '../inversify/inversify.types';

export default class SignalR {
	@inject(Types.SignalRSubscribers) private _signalRSubscribers!: SignalRSubscribers;
	private _connection: HubConnection | undefined;

	constructor() {
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
