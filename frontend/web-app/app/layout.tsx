import './globals.css';

import type { Metadata } from 'next';
import Navbar from './nav/NavBar';
import SignalRProvider from './providers/SignalRProvider';
import ToasterProvider from './providers/ToasterProvider';
import { getCurrentUser } from './actions/authActions';

export const metadata: Metadata = {
  title: 'Car Auction App',
  description: 'A simple car auction app',
};

export default async function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  const user = await getCurrentUser();
  return (
    <html lang='en'>
      <body>
        <ToasterProvider />
        <Navbar />
        <main className='container mx-auto px-5 pt-10'>
          <SignalRProvider user={user}>{children}</SignalRProvider>
        </main>
      </body>
    </html>
  );
}
