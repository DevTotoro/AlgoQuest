import { CompletionStatus } from '@prisma/client';
import { NextRequest, NextResponse } from 'next/server';

import { db } from '~/lib/db';
import { HttpCode, httpResponse, validateParams } from '~/lib/http';
import { getHighScoreSchema } from '~/lib/schemas/algorithms.schema';

const GET = async (request: NextRequest) => {
  const searchParams = request.nextUrl.searchParams;
  const takeParam = searchParams.get('take');
  const algorithmParam = searchParams.get('algorithm');

  const params = validateParams({ take: takeParam, algorithm: algorithmParam }, getHighScoreSchema);

  if (params instanceof NextResponse) {
    return params;
  }

  let take: number;
  try {
    take = parseInt(params.take, 10);
  } catch (error) {
    return httpResponse(HttpCode.BAD_REQUEST, 'Invalid take parameter');
  }

  try {
    const data = await db.algorithm.findMany({
      where: {
        type: params.algorithm,
        status: CompletionStatus.SUCCESS,
      },
      orderBy: {
        time: 'asc',
      },
      include: {
        sessions: {
          select: {
            id: true,
            username: true,
          },
        },
      },
      take,
    });

    return NextResponse.json(data);
  } catch (error) {
    return httpResponse(HttpCode.INTERNAL_SERVER_ERROR);
  }
};

export { GET };
