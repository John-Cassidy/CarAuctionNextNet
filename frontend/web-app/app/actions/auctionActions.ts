'use server';

import { Auction, PagedResult } from '@/types';

export async function getData(
  pageNumber: number = 1
): Promise<PagedResult<Auction>> {
  console.log(`http://localhost:6001/search/?pageNumber=${pageNumber}`);
  const response = await fetch(
    `http://localhost:6001/search/?pageNumber=${pageNumber}`
  );
  if (!response.ok) {
    throw new Error('Network response was not ok');
  }
  return await response.json();
}
