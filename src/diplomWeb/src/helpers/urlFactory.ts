export const URL_FACTORY = {
	token: 'connect/token',

	usersCurrent: 'users/current',
	bookingsOverdue: 'bookings/overdue',
	bookingsOverdueList: 'bookings/overdue/list',

	files: 'files/',

	sync: (requestAddress: string): string => `${requestAddress}/sync`,
	tableDataInfo: (requestAddress: string, id: string): string => `${requestAddress}/${id}`,
	tableDataPhoto: (fileId: string): string => `${URL_FACTORY.files}${fileId}`,

	usernames: 'users/usernames',

	deviceHistory: (id: string): string => `devices/${id}/history`,
};
