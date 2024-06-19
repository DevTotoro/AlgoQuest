import Link from 'next/link';
import { Inter } from 'next/font/google';

const fontSans = Inter({
  subsets: ['latin'],
  variable: '--font-sans',
});

const NotFoundPage = () => {
  return (
    <div className={`min-h-screen font-sans antialiased ${fontSans.variable}`}>
      <div className='flex h-screen flex-col items-center justify-center space-y-8'>
        <div className='flex flex-col items-center justify-center space-y-2'>
          <h1 className='text-4xl font-bold'>404</h1>
          <p className='text-lg font-light'>Page not found</p>
        </div>

        <Link href='/' className='underline'>
          Home
        </Link>
      </div>
    </div>
  );
};

export default NotFoundPage;
