import { useMemo } from 'react';
import { getContainer } from '../inversify/inversify.getContainer';

export const useInject = <T>(containerId: string | symbol): T => {
	return useMemo(() => getContainer<T>(containerId), [containerId]);
};
