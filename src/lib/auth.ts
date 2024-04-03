import type { NextRequest } from 'next/server';
import { env } from '~/lib/env';

export const isAuthorized = (request: NextRequest) => {
  const apiKey = request.headers.get('Authorization')?.replace('Bearer ', '');

  return apiKey === env.API_KEY;
};
