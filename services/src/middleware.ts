import { type NextRequest, NextResponse } from 'next/server';

import { isAuthorized } from '~/lib/auth';
import { HttpCode, httpResponse } from '~/lib/http';

export const middleware = (request: NextRequest) => {
  if (!isAuthorized(request)) {
    return httpResponse(HttpCode.UNAUTHORIZED);
  }

  return NextResponse.next();
};

export const config = {
  matcher: '/api/:path*',
};
