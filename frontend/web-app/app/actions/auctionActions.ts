'use server';

import { Auction, PagedResult } from '@/types';

import { FieldValues } from 'react-hook-form';
import { fetchWrapper } from '../lib/fetchWrapper';
import { getTokenWorkaround } from './authActions';

export async function getData(query: string): Promise<PagedResult<Auction>> {
  return await fetchWrapper.get(`search/${query}`);
}

export async function updateAuctionTest(): Promise<
  string | { status: number; message: string }
> {
  const data = {
    mileage: Math.floor(Math.random() * 100000) + 1,
  };

  const token = await getTokenWorkaround();

  return await fetchWrapper.put(
    'auctions/afbee524-5972-4075-8800-7d1f9d7b0a0c',
    data
  );
}

export async function createAuction(data: FieldValues) {
  return await fetchWrapper.post('auctions', data);
}

export async function getDetailedViewData(id: string): Promise<Auction> {
  return await fetchWrapper.get(`auctions/${id}`);
}
