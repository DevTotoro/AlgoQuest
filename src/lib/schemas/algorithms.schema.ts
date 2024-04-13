import { z } from 'zod';
import { AlgorithmType } from '@prisma/client';

export type CreateAlgorithmSchema = z.infer<typeof createAlgorithmSchema>;
export const createAlgorithmSchema = z.object({
  type: z.nativeEnum(AlgorithmType),
  randomValues: z.boolean(),
  initialValues: z.array(z.number()).optional(),
});
