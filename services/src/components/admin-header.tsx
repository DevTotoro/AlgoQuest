import Link from 'next/link';

export const AdminHeader = () => {
  return (
    <div className='navbar sticky top-0 max-w-3xl bg-base-100'>
      <div className='flex-1'>
        <Link href='/admin' className='btn btn-ghost whitespace-nowrap text-xl'>
          AlgoQuest Services
        </Link>
      </div>

      <div className='flex-none'>
        <ul className='menu menu-horizontal px-1'>
          <li>
            <Link href='/admin'>Statistics</Link>
          </li>

          <li>
            <Link href='/admin/algorithms'>Algorithms</Link>
          </li>
        </ul>
      </div>
    </div>
  );
};
