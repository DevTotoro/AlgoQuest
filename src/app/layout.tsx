import '~/styles/globals.css';

import type { Metadata } from 'next';
import { Inter } from 'next/font/google';

const fontSans = Inter({
  subsets: ['latin'],
  variable: '--font-sans',
});

export const metadata: Metadata = {
  title: 'AlgoQuest Services',
  description: 'A collection of services for the AlgoQuest project',
};

const RootLayout = ({ children }: { children: React.ReactNode }) => {
  return (
    <html lang='en'>
      <body className={`min-h-screen font-sans antialiased ${fontSans.variable}`}>{children}</body>
    </html>
  );
};

export default RootLayout;
