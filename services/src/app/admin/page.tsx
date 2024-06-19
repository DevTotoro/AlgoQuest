import type { Metadata } from 'next';

import { db } from '~/lib/db';

export const metadata: Metadata = {
  title: 'AlgoQuest Services • Admin • Statistics',
};

const AdminStatisticsPage = async () => {
  const sessionCount = await db.session.count();
  const hostsCreatedCount = await db.log.count({ where: { type: 'HOST_CREATED' } });

  const guidedStartedCount = await db.log.count({
    where: {
      AND: {
        type: 'GAME_STARTED',
        gameMode: 'GUIDED',
      },
    },
  });
  const timeTrialsStartedCount = await db.log.count({
    where: {
      AND: {
        type: 'GAME_STARTED',
        gameMode: 'TIME_TRIAL',
      },
    },
  });
  const timeTrialsWonCount = await db.log.count({
    where: {
      AND: {
        type: 'GAME_WON',
        gameMode: 'TIME_TRIAL',
      },
    },
  });

  const bubbleSortAverageTime = await db.timeTrial.aggregate({
    _avg: {
      time: true,
    },
    where: {
      type: 'BUBBLE_SORT',
    },
  });
  const selectionSortAverageTime = await db.timeTrial.aggregate({
    _avg: {
      time: true,
    },
    where: {
      type: 'SELECTION_SORT',
    },
  });

  return (
    <div className='flex h-full w-full flex-col items-center gap-4'>
      {/* Sessions & Worlds */}
      <div className='stats w-full border shadow'>
        <div className='stat'>
          <div className='stat-title'>Sessions</div>
          <div className='stat-value'>{sessionCount}</div>
        </div>

        <div className='stat'>
          <div className='stat-title'>Worlds created</div>
          <div className='stat-value'>{hostsCreatedCount}</div>
        </div>
      </div>

      {/* Games */}
      <div className='stats w-full border shadow'>
        <div className='stat'>
          <div className='stat-title'>
            <b>Guided</b> started
          </div>
          <div className='stat-value'>{guidedStartedCount}</div>
        </div>

        <div className='stat'>
          <div className='stat-title'>
            <b>Time Trials</b> started
          </div>
          <div className='stat-value'>{timeTrialsStartedCount}</div>
        </div>

        <div className='stat'>
          <div className='stat-title'>
            <b>Time Trials</b> won
          </div>
          <div className='stat-value'>{timeTrialsWonCount}</div>
          <div className='stat-desc'>
            <b>
              {timeTrialsStartedCount > 0
                ? `${Math.round((timeTrialsWonCount / timeTrialsStartedCount) * 100).toString()} %`
                : '0 %'}
            </b>{' '}
            of total Time Trials
          </div>
        </div>
      </div>

      {/* Times */}
      <div className='stats w-full border shadow'>
        <div className='stat'>
          <div className='stat-title'>
            Average time for <b>Bubble Sort</b>
          </div>
          <div className='stat-value'>{((bubbleSortAverageTime._avg.time ?? 0) / 1000).toFixed(2)} s</div>
          <div className='stat-desc'>Counted based on Time Trials</div>
        </div>

        <div className='stat'>
          <div className='stat-title'>
            Average time for <b>Selection Sort</b>
          </div>
          <div className='stat-value'>{((selectionSortAverageTime._avg.time ?? 0) / 1000).toFixed(2)} s</div>
          <div className='stat-desc'>Counted based on Time Trials</div>
        </div>
      </div>
    </div>
  );
};

export default AdminStatisticsPage;
