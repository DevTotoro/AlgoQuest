import { z } from 'zod';
import { AlgorithmType, GameMode, LogType } from '@prisma/client';

export const createLogSchema = z.object({
  type: z.nativeEnum(LogType),
  message: z.string(),

  algorithmType: z.nativeEnum(AlgorithmType).nullish(),
  gameMode: z.nativeEnum(GameMode).nullish(),
  containerValues: z.array(z.number()).nullish(),

  sessionId: z.string(),
});
