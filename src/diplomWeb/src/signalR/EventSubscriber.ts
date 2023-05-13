import { makeAutoObservable } from 'mobx';
import { IEventSubscriber } from '../models/interfaces/IEventSubscriber';
import { injectable } from 'inversify';

@injectable()
export default class EventSubscriber<EventData> implements IEventSubscriber<EventData> {
	private _method: ((data?: EventData | undefined) => void) | undefined;

	constructor() {
		makeAutoObservable(this);
	}

	Add(handler: (data?: EventData | undefined) => void): void {
		this._method = handler;
	}

	Delete(handler: (data?: EventData | undefined) => void): void {
		this._method = undefined;
	}

	TriggerEvent(data?: EventData | undefined): void {
		if (this._method) {
			this._method(data);
		}
	}
}
