import { NextResponse } from 'next/server';

const GET = () => {
  return NextResponse.json({ status: 'ok' });
};

export { GET };
