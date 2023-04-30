import container from './inversify.config';

export const getContainer = <T>(id: string | symbol): T => container.get<T>(id);
