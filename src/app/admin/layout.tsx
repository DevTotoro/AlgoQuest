import { AdminHeader } from '~/components/admin-header';

const AdminLayout = ({ children }: { children: React.ReactNode }) => {
  return (
    <div className='flex h-screen flex-col items-center'>
      <AdminHeader />

      <div className='flex w-full max-w-3xl flex-1 p-4'>{children}</div>
    </div>
  );
};

export default AdminLayout;
