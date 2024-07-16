import './globals.css';

import type { Metadata } from 'next';
import Navbar from './nav/NavBar';

export const metadata: Metadata = {
  title: 'Car Auction App',
  description: 'A simple car auction app',
};

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <html lang='en'>
      <body>
        <Navbar />
        <main className='container mx-auto px-5 pt-10'>{children}</main>
      </body>
    </html>
  );
}