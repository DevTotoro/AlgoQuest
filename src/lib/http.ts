import { NextResponse } from 'next/server';

export enum HttpCode {
  UNAUTHORIZED = 401,
}

const responseMessages: { [key in HttpCode]: string } = {
  [HttpCode.UNAUTHORIZED]: 'Unauthorized',
};

export const httpResponse = (status: HttpCode, message?: string) =>
  NextResponse.json({ status, message: message ?? responseMessages[status] }, { status });
