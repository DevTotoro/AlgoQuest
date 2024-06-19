import { z } from 'zod';
import { AlgorithmType, GameMode } from '@prisma/client';

export const createContainerInteractionSchema = z.object({
  containerIndex: z.number(),
  receivedValue: z.number().optional(),
  placedValue: z.number().optional(),
  isValid: z.boolean(),
  containerValues: z.array(z.number()),
  algorithmType: z.nativeEnum(AlgorithmType),
  gameMode: z.nativeEnum(GameMode),
  sessionId: z.string(),
});
