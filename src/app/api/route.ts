import { NextResponse } from 'next/server';

const handler = () => {
  return NextResponse.json({ status: 'ok' });
};

export { handler as GET };
