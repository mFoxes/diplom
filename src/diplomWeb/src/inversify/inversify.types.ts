export const Types = {
	// service
	AuthService: Symbol('AuthService'),
	LocalStorageService: Symbol('LocalStorageService'),
	CurrentEmployeeService: Symbol('CurrentEmployeeService'),
	DownloadableImageService: Symbol('DownloadableImageService'),
	// service end

	// stores
	AuthStore: Symbol('AuthStore'),
	DashboardStore: Symbol('DashboardStore'),
	DevicesStore: Symbol('DevicesStore'),
	EmployeeStore: Symbol('EmployeeStore'),
	GeneralStore: Symbol('GeneralStore'),

	PhotosCacheStore: Symbol('PhotosCacheStore'),
	// stores end

	// signalR
	SignalR: Symbol('SignalR'),
	SignalRSubscribers: Symbol('SignalRSubscribers'),
	// signalR end
};
