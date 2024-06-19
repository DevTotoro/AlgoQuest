import { z } from 'zod';
import { AlgorithmType } from '@prisma/client';

export type CreateAlgorithmSchema = z.infer<typeof createAlgorithmSchema>;
export const createAlgorithmSchema = z.object({
  type: z.nativeEnum(AlgorithmType),
  numberOfValues: z.number().min(3).max(30),
  minValue: z.number().min(3).max(99),
  maxValue: z.number().min(3).max(99),
});

export type UpdateAlgorithmSchema = z.infer<typeof updateAlgorithmSchema>;
export const updateAlgorithmSchema = z.object({
  id: z.string(),
  type: createAlgorithmSchema.shape.type.optional(),
  numberOfValues: createAlgorithmSchema.shape.numberOfValues.optional(),
  minValue: createAlgorithmSchema.shape.minValue.optional(),
  maxValue: createAlgorithmSchema.shape.maxValue.optional(),
});
