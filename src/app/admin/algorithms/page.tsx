import type { Metadata } from 'next';

export const metadata: Metadata = {
  title: 'AlgoQuest Services • Admin • Algorithms',
};

const AdminAlgorithmsPage = () => {
  return (
    <div className='flex h-full flex-col items-center justify-center space-y-4'>
      <h1>Algorithms</h1>
    </div>
  );
};

export default AdminAlgorithmsPage;
