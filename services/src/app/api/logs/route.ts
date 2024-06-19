import { NextRequest, NextResponse } from 'next/server';

import { db } from '~/lib/db';
import { HttpCode, httpResponse, validateBody } from '~/lib/http';
import { createLogSchema } from '~/lib/schemas/logs.schema';

const POST = async (request: NextRequest) => {
  const body: unknown = await request.json();
  const data = validateBody(body, createLogSchema);

  if (data instanceof NextResponse) {
    return data;
  }

  // Check if session exists
  try {
    const session = await db.session.findFirst({
      where: {
        id: data.sessionId,
      },
    });

    if (!session) {
      return httpResponse(HttpCode.NOT_FOUND, 'Session not found');
    }
  } catch (error) {
    return httpResponse(HttpCode.INTERNAL_SERVER_ERROR);
  }

  try {
    await db.log.create({
      data: {
        ...data,
        containerValues: data.containerValues?.length ? data.containerValues : undefined,
        sessionId: undefined,
        session: {
          connect: {
            id: data.sessionId,
          },
        },
      },
    });

    return httpResponse(HttpCode.CREATED);
  } catch (error) {
    return httpResponse(HttpCode.INTERNAL_SERVER_ERROR);
  }
};

export { POST };
