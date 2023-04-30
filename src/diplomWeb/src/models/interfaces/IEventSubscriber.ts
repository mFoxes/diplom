export interface IEventSubscriber<EventData> {
	Add(handler: { (data?: EventData): void }): void;
	Delete(handler: { (data?: EventData): void }): void;
	TriggerEvent(data?: EventData): void;
}
