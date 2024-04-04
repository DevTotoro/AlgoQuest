import { NextRequest, NextResponse } from 'next/server';

import { db } from '~/lib/db';
import { HttpCode, httpResponse, validateBody } from '~/lib/http';
import { createSessionSchema } from '~/lib/schemas/sessions.schema';

const GET = async () => {
  try {
    const data = await db.session.findMany();

    return NextResponse.json(data);
  } catch (error) {
    return httpResponse(HttpCode.INTERNAL_SERVER_ERROR);
  }
};

const POST = async (request: NextRequest) => {
  const body: unknown = await request.json();
  const data = validateBody(body, createSessionSchema);

  if (data instanceof NextResponse) {
    return data;
  }

  try {
    const session = await db.session.create({ data });

    return NextResponse.json(session);
  } catch (error) {
    return httpResponse(HttpCode.INTERNAL_SERVER_ERROR);
  }
};

export { GET, POST };
