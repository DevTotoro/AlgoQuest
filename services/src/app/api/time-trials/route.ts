import { NextRequest, NextResponse } from 'next/server';

import { db } from '~/lib/db';
import { HttpCode, httpResponse, validateBody, validateParams } from '~/lib/http';
import { getTimeTrialsParamsSchema, createTimeTrialSchema } from '~/lib/schemas/time-trials.schema';

const GET = async (request: NextRequest) => {
  const searchParams = request.nextUrl.searchParams;

  const params = validateParams(searchParams, getTimeTrialsParamsSchema);

  if (params instanceof NextResponse) {
    return params;
  }

  const numberOfValuesDiff = 2;
  const requiredMovesDiff = 2;

  const take = params.take ? parseInt(params.take, 10) : 10;
  const numberOfValues = params.numberOfValues ? parseInt(params.numberOfValues, 10) : undefined;
  const requiredMoves = params.requiredMoves ? parseInt(params.requiredMoves, 10) : undefined;

  try {
    const data = await db.timeTrial.findMany({
      take,
      include: {
        sessions: {
          select: {
            id: true,
            username: true,
          },
        },
      },
      where: {
        type: params.type,
        numberOfValues: numberOfValues
          ? {
              gte: numberOfValues - numberOfValuesDiff,
              lte: numberOfValues + numberOfValuesDiff,
            }
          : undefined,
        requiredMoves: requiredMoves
          ? {
              gte: requiredMoves - requiredMovesDiff,
              lte: requiredMoves + requiredMovesDiff,
            }
          : undefined,
      },
      orderBy: {
        time: 'asc',
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
    await db.timeTrial.create({
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

    return httpResponse(HttpCode.CREATED);
  } catch (error) {
    return httpResponse(HttpCode.INTERNAL_SERVER_ERROR);
  }
};

export { GET, POST };
