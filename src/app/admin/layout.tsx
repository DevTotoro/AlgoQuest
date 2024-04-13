import { AdminHeader } from '~/components/admin-header';

const AdminLayout = ({ children }: { children: React.ReactNode }) => {
  return (
    <div className='flex h-screen flex-col items-center'>
      <AdminHeader />

      <div className='flex flex-1'>{children}</div>
    </div>
  );
};

export default AdminLayout;
