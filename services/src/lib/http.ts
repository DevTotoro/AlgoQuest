import { NextResponse } from 'next/server';
import { Schema, ZodIssue } from 'zod';

export enum HttpCode {
  OK = 200,
  CREATED = 201,

  BAD_REQUEST = 400,
  UNAUTHORIZED = 401,
  NOT_FOUND = 404,

  INTERNAL_SERVER_ERROR = 500,
}

const responseMessages: { [key in HttpCode]: string } = {
  [HttpCode.OK]: 'OK',
  [HttpCode.CREATED]: 'Created',

  [HttpCode.BAD_REQUEST]: 'Bad Request',
  [HttpCode.UNAUTHORIZED]: 'Unauthorized',
  [HttpCode.NOT_FOUND]: 'Not Found',

  [HttpCode.INTERNAL_SERVER_ERROR]: 'Internal Server Error',
};

export const httpResponse = (status: HttpCode, message?: string, errors?: ZodIssue[]) =>
  NextResponse.json({ status, message: message ?? responseMessages[status], errors }, { status });

export const validateBody = <T>(body: unknown, schema: Schema<T>): T | NextResponse => {
  const result = schema.safeParse(body);

  if (!result.success) {
    return httpResponse(HttpCode.BAD_REQUEST, undefined, result.error.errors);
  }

  return result.data;
};

export const validateParams = <T>(params: URLSearchParams, schema: Schema<T>): T | NextResponse => {
  const result = schema.safeParse(Object.fromEntries(Array.from(params.entries())));

  if (!result.success) {
    return httpResponse(HttpCode.BAD_REQUEST, undefined, result.error.errors);
  }

  return result.data;
};
