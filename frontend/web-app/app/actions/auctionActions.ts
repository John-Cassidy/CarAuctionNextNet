'use server';

import { Auction, PagedResult } from '@/types';

export async function getData(query: string): Promise<PagedResult<Auction>> {
  const response = await fetch(`http://localhost:6001/search/${query}`);
  if (!response.ok) {
    throw new Error('Network response was not ok');
  }
  return await response.json();
}

export async function updateAuctionTest(): Promise<
  string | { status: number; message: string }
> {
  const data = {
    mileage: Math.floor(Math.random() * 100000) + 1,
  };

  const response = await fetch(
    `http://localhost:6001/auctions/afbee524-5972-4075-8800-7d1f9d7b0a0c`,
    {
      method: 'PUT',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(data),
    }
  );

  if (!response.ok)
    return { status: response.status, message: response.statusText };

  return response.statusText;
}
