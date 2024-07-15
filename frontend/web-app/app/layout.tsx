import './globals.css';

import type { Metadata } from 'next';

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
      <body>{children}</body>
    </html>
  );
}
