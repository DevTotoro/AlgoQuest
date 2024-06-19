import type { Metadata } from 'next';

export const metadata: Metadata = {
  title: 'AlgoQuest Services',
  description: 'A collection of services for the AlgoQuest project',
};

const HomePage = () => {
  return (
    <div className='flex h-screen flex-col items-center justify-center space-y-4'>
      <h1 className='text-3xl font-semibold'>Welcome to AlgoQuest Services</h1>
      <p className='text-lg font-light'>
        You are probably looking for the{' '}
        <a
          href='https://github.com/DevTotoro/AlgoQuest'
          className='underline'
          target='_blank'
          rel='noopener noreferrer'
        >
          AlgoQuest Project
        </a>
      </p>
    </div>
  );
};

export default HomePage;
