import { NextRequest, NextResponse } from 'next/server';

import { db } from '~/lib/db';
import { HttpCode, httpResponse, validateBody } from '~/lib/http';
import { createAlgorithmSchema, updateAlgorithmSchema } from '~/lib/schemas/algorithms.schema';

const GET = async () => {
  try {
    const data = await db.algorithm.findMany({
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
  const data = validateBody(body, createAlgorithmSchema);

  if (data instanceof NextResponse) {
    return data;
  }

  try {
    await db.algorithm.create({
      data,
    });

    return httpResponse(HttpCode.CREATED);
  } catch (error) {
    return httpResponse(HttpCode.INTERNAL_SERVER_ERROR);
  }
};

const PUT = async (request: NextRequest) => {
  const body: unknown = await request.json();
  const data = validateBody(body, updateAlgorithmSchema);

  if (data instanceof NextResponse) {
    return data;
  }

  try {
    await db.algorithm.update({
      where: {
        id: data.id,
      },
      data: {
        ...data,
      },
    });

    return httpResponse(HttpCode.OK);
  } catch (error) {
    return httpResponse(HttpCode.INTERNAL_SERVER_ERROR);
  }
};

export { GET, POST, PUT };
