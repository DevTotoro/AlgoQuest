import { z } from 'zod';
import { AlgorithmType } from '@prisma/client';

export const createTimeTrialSchema = z.object({
  type: z.nativeEnum(AlgorithmType),
  time: z.number().positive(),
  numberOfValues: z.number().positive(),
  requiredMoves: z.number().positive(),
  sessions: z.array(z.string()),
});
