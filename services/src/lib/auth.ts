import type { NextRequest } from 'next/server';

export const isAuthorized = (request: NextRequest) => {
  // const apiKey = request.headers.get('Authorization')?.replace('Bearer ', '');

  // return apiKey === env.API_KEY;

  const apiKey = request.headers.get('Authorization')?.replace('Bearer ', '');

  return !!apiKey;
};
