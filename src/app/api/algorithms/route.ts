import { NextRequest, NextResponse } from 'next/server';

import { db } from '~/lib/db';
import { HttpCode, httpResponse, validateBody } from '~/lib/http';
import { createAlgorithmSchema } from '~/lib/schemas/algorithms.schema';

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

// const POST = async (request: NextRequest) => {
//   const body: unknown = await request.json();
//   const data = validateBody(body, createAlgorithmSchema);

//   if (data instanceof NextResponse) {
//     return data;
//   }

//   // Check if all sessions exist
//   try {
//     const sessions = await db.session.findMany({
//       select: {
//         id: true,
//       },
//       where: {
//         id: {
//           in: data.sessions,
//         },
//       },
//     });

//     if (sessions.length !== data.sessions.length) {
//       return httpResponse(HttpCode.NOT_FOUND, 'One or more sessions not found');
//     }
//   } catch (error) {
//     return httpResponse(HttpCode.INTERNAL_SERVER_ERROR);
//   }

//   // Create algorithm
//   try {
//     const algorithm = await db.algorithm.create({
//       data: {
//         ...data,
//         sessions: {
//           connect: data.sessions.map((id) => ({ id })),
//         },
//       },
//       include: {
//         sessions: {
//           select: {
//             id: true,
//             username: true,
//           },
//         },
//       },
//     });

//     return NextResponse.json(algorithm);
//   } catch (error) {
//     return httpResponse(HttpCode.INTERNAL_SERVER_ERROR);
//   }
// };

export { GET, POST };
