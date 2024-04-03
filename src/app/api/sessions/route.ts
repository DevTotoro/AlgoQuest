import { NextResponse } from 'next/server';
import { db } from '~/lib/db';
import { HttpCode, httpResponse } from '~/lib/http';

const handler = async () => {
  try {
    const data = await db.session.findMany();

    return NextResponse.json({ data });
  } catch (error) {
    return httpResponse(HttpCode.INTERNAL_SERVER_ERROR);
  }
};

export { handler as GET };
