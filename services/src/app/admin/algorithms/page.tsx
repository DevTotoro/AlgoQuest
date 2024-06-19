import type { Metadata } from 'next';
import { AlgorithmType } from '@prisma/client';

import { getAlgorithmDisplayName } from '~/lib/utils';
import { db } from '~/lib/db';
import { AlgorithmConfig } from '~/components/algorithm-config';

export const metadata: Metadata = {
  title: 'AlgoQuest Services • Admin • Algorithms',
};

const AdminAlgorithmsPage = async () => {
  const dbAlgorithms = await db.algorithm.findMany({
    orderBy: {
      createdAt: 'desc',
    },
  });

  return (
    <main className='flex h-full w-full flex-col items-center space-y-4'>
      {Object.values(AlgorithmType).map((algorithm) => {
        const dbAlgorithm = dbAlgorithms.find((dbAlgorithm) => dbAlgorithm.type === algorithm);

        return (
          <div key={algorithm} className='card w-full bg-base-100 shadow-xl'>
            <div className='card-body'>
              <h2 className='card-title'>{getAlgorithmDisplayName(algorithm)}</h2>

              <AlgorithmConfig type={algorithm} algorithm={dbAlgorithm} />
            </div>
          </div>
        );
      })}
    </main>
  );
};

export default AdminAlgorithmsPage;
