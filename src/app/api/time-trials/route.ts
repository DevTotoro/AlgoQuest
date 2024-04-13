import { NextRequest, NextResponse } from 'next/server';

import { db } from '~/lib/db';
import { HttpCode, httpResponse, validateBody } from '~/lib/http';
import { createTimeTrialSchema } from '~/lib/schemas/time-trials.schema';

const GET = async () => {
  try {
    const data = await db.timeTrial.findMany({
      orderBy: {
        createdAt: 'desc',
      },
    });

    return NextResponse.json(data);
  } catch (error) {
    return httpResponse(HttpCode.INTERNAL_SERVER_ERROR);
  }
};

const POST = async (request: NextRequest) => {
  const body: unknown = await request.json();
  const data = validateBody(body, createTimeTrialSchema);

  if (data instanceof NextResponse) {
    return data;
  }

  // Check if all sessions exist
  try {
    const sessions = await db.session.findMany({
      select: {
        id: true,
      },
      where: {
        id: {
          in: data.sessions,
        },
      },
    });

    if (sessions.length !== data.sessions.length) {
      return httpResponse(HttpCode.NOT_FOUND, 'One or more sessions not found');
    }
  } catch (error) {
    return httpResponse(HttpCode.INTERNAL_SERVER_ERROR);
  }

  // Create time trial
  try {
    const timeTrial = await db.timeTrial.create({
      data: {
        ...data,
        sessions: {
          connect: data.sessions.map((id) => ({ id })),
        },
      },
      include: {
        sessions: {
          select: {
            id: true,
            username: true,
          },
        },
      },
    });

    return NextResponse.json(timeTrial);
  } catch (error) {
    return httpResponse(HttpCode.INTERNAL_SERVER_ERROR);
  }
};

export { GET, POST };
