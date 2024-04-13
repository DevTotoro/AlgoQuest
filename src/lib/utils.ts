import { AlgorithmType } from '@prisma/client';

/* eslint-disable @typescript-eslint/no-unused-vars */
export const assertUnreachable = (x: never): never => {
  throw new Error('Unreachable code reached');
};

export const getAlgorithmDisplayName = (algorithm: AlgorithmType) => {
  switch (algorithm) {
    case AlgorithmType.BUBBLE_SORT:
      return 'Bubble Sort';
    case AlgorithmType.SELECTION_SORT:
      return 'Selection Sort';
    default:
      assertUnreachable(algorithm);
  }
};

export const copyToClipboard = async (text: string) => {
  await navigator.clipboard.writeText(text);
};
