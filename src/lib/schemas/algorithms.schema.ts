import { z } from 'zod';
import { AlgorithmType, CompletionStatus } from '@prisma/client';

export const createAlgorithmSchema = z.object({
  type: z.nativeEnum(AlgorithmType),
  status: z.nativeEnum(CompletionStatus),
  time: z.number(),
  sessions: z.array(z.string()),
});
