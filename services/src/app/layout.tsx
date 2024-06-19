import '~/styles/globals.css';

import { Inter } from 'next/font/google';

const fontSans = Inter({
  subsets: ['latin'],
  variable: '--font-sans',
});

const RootLayout = ({ children }: { children: React.ReactNode }) => {
  return (
    <html lang='en'>
      <body className={`min-h-screen font-sans antialiased ${fontSans.variable}`}>{children}</body>
    </html>
  );
};

export default RootLayout;
