import { z } from 'zod';

export const createSessionSchema = z.object({
  username: z
    .string()
    .min(3, { message: 'Username must be at least 3 characters long' })
    .max(255, { message: 'Username must be at most 255 characters long' }),
});
