import { NextResponse } from 'next/server';

export enum HttpCode {
  UNAUTHORIZED = 401,

  INTERNAL_SERVER_ERROR = 500,
}

const responseMessages: { [key in HttpCode]: string } = {
  [HttpCode.UNAUTHORIZED]: 'Unauthorized',

  [HttpCode.INTERNAL_SERVER_ERROR]: 'Internal Server Error',
};

export const httpResponse = (status: HttpCode, message?: string) =>
  NextResponse.json({ status, message: message ?? responseMessages[status] }, { status });
