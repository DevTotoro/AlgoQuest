import { NextRequest, NextResponse } from 'next/server';
import { AlgorithmType } from '@prisma/client';

import { db } from '~/lib/db';
import { HttpCode, httpResponse } from '~/lib/http';

interface Params {
  params: {
    type: AlgorithmType;
  };
}

const GET = async (_: NextRequest, { params }: Params) => {
  const { type } = params;

  if (!Object.values(AlgorithmType).includes(type)) {
    return httpResponse(HttpCode.BAD_REQUEST, 'Invalid algorithm type');
  }

  try {
    const data = await db.algorithm.findFirst({
      where: {
        type: params.type,
      },
      orderBy: {
        createdAt: 'desc',
      },
    });

    if (!data) {
      return httpResponse(HttpCode.NOT_FOUND, 'Algorithm not found');
    }

    return NextResponse.json(data);
  } catch (error) {
    return httpResponse(HttpCode.INTERNAL_SERVER_ERROR);
  }
};

export { GET };
